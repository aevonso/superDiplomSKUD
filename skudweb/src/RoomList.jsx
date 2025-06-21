import React, { useEffect, useState, useRef } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import apiClient from './apiClient';
import logo from './assets/natk-logo.png';
import QRCode from 'react-qr-code';
import html2canvas from 'html2canvas';
import jsPDF from 'jspdf';
import { fetchPostsWithAccess } from './accessMatrixApi';
import './ListPage.css';

export default function RoomList() {
    const [rooms, setRooms] = useState([]);
    const [accessMap, setAccessMap] = useState({}); // { [roomId]: [posts...] }
    const [q, setQ] = useState('');
    const [collapsed, setCollapsed] = useState(
        () => JSON.parse(localStorage.getItem('sidebar-collapsed') || 'false')
    );
    const navigate = useNavigate();
    const batchRef = useRef();

    // 1) Загрузить список всех помещений
    useEffect(() => {
        apiClient.get('/api/Room').then(r => {
            setRooms(r.data);
        });
    }, []);

    // 2) Для каждого помещения подгружаем матрицу доступа
    useEffect(() => {
        rooms.forEach(rm => {
            fetchPostsWithAccess(rm.id)
                .then(all => {
                    // фильтруем только те должности, у которых hasAccess=true
                    const allowed = all.filter(p => p.hasAccess);
                    setAccessMap(m => ({ ...m, [rm.id]: allowed }));
                })
                .catch(() => {
                    // игнорируем ошибку одной комнаты
                    console.error(`Не удалось загрузить доступы для помещения ${rm.id}`);
                });
        });
    }, [rooms]);

    // 3) Фильтр по поиску
    const filtered = rooms.filter(rm =>
        rm.name.toLowerCase().includes(q.toLowerCase()) ||
        rm.floor?.name.toLowerCase().includes(q.toLowerCase())
    );

    // 4) Генерация PDF: только для тех комнат, где есть хотя бы одна должность
    const generateAllQR = async () => {
        const toPrint = rooms.filter(rm =>
            Array.isArray(accessMap[rm.id]) && accessMap[rm.id].length > 0
        );
        if (!toPrint.length) {
            return alert('Нет помещений с доступными должностями');
        }

        // ждём, пока React отрисует скрытые страницы
        await new Promise(res => setTimeout(res, 100));

        const pdf = new jsPDF('p', 'pt', 'a4');
        const pages = batchRef.current.children;
        let pageIndex = 0;

        for (let el of pages) {
            const id = Number(el.dataset.roomId);
            if (!toPrint.find(r => r.id === id)) continue;

            const canvas = await html2canvas(el, { scale: 2 });
            const img = canvas.toDataURL('image/png');
            const w = pdf.internal.pageSize.getWidth();
            const h = (canvas.height * w) / canvas.width;

            if (pageIndex > 0) pdf.addPage();
            pdf.addImage(img, 'PNG', 0, 0, w, h);
            pageIndex++;
        }

        pdf.save('all_rooms_qr.pdf');
    };

    return (
        <div className="Dashboard">
            <header className="Header">
                <img src={logo} alt="НАТК" className="Header-logo" />
                <span className="Header-title">Помещения</span>
            </header>

            <div className="Body">
                <aside className={`Sidebar ${collapsed ? 'collapsed' : ''}`}>
                    <button
                        className="Burger SidebarBurger"
                        onClick={() => {
                            const nc = !collapsed;
                            setCollapsed(nc);
                            localStorage.setItem('sidebar-collapsed', JSON.stringify(nc));
                        }}
                        aria-label="Toggle sidebar"
                    >
                        <span /><span /><span />
                    </button>
                    <nav>
                        <a href="/employees">Сотрудники</a>
                        <a href="/devices">Устройства</a>
                        <a href="/accessmatrix">Матрица доступа</a>
                        <a href="/dashboard">Лог событий</a>
                        <a href="/rooms" className="active">Помещения</a>
                        <a href="/divisions">Подразделения</a>
                        <a href="/posts">Должности</a>
                        <a href="/reports">Отчёты</a>
                        <a href="/setting">Настройки</a>
                    </nav>
                </aside>

                <main className="Main ListPage">
                    <div className="Search">
                        <input
                            type="text"
                            placeholder="Поиск"
                            value={q}
                            onChange={e => setQ(e.target.value)}
                        />
                        <button className="AddBtn" onClick={generateAllQR}>
                            Генерировать QR для всех
                        </button>
                    </div>

                    <div className="TableWrapper">
                        <table className="ListTable">
                            <thead>
                                <tr><th>ID</th><th>Номер помещения</th><th>Этаж</th></tr>
                            </thead>
                            <tbody>
                                {filtered.map(rm => (
                                    <tr
                                        key={rm.id}
                                        onClick={() => navigate(`/rooms/${rm.id}`)}
                                        style={{ cursor: 'pointer' }}
                                    >
                                        <td>{rm.id}</td>
                                        <td>{rm.name}</td>
                                        <td>{rm.floor?.name}</td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    </div>

                    <div className="Actions">
                        <Link to="/rooms/new" className="AddBtn">
                            Добавить помещение
                        </Link>
                    </div>
                </main>
            </div>

            {/* Скрытый контейнер: для каждой комнаты отрисовываем QR-страницу */}
            <div className="QRBatchContainer" ref={batchRef}>
                {rooms.map(rm => {
                    // отрисовываем QR-код для всех комнат, но в PDF попадут только те, у которых есть доступные должности
                    return (
                        <div
                            key={rm.id}
                            className="qr-page"
                            data-room-id={rm.id}
                        >
                            <h2 className="qr-header">QR-код для помещения №{rm.name}</h2>
                            <div className="qr-wrapper">
                                <QRCode value={JSON.stringify({ roomId: rm.id })} size={150} />
                            </div>
                        </div>
                    );
                })}
            </div>
        </div>
    );
}
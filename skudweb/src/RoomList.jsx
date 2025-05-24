import React, { useEffect, useState, useRef } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import apiClient from './apiClient';
import logo from './assets/natk-logo.png';
import QRCode from 'react-qr-code';
import html2canvas from 'html2canvas';
import jsPDF from 'jspdf';
import './ListPage.css';

export default function RoomList() {
    const [rooms, setRooms] = useState([]);
    const [q, setQ] = useState('');
    const [collapsed, setCollapsed] = useState(
        () => JSON.parse(localStorage.getItem('sidebar-collapsed') || 'false')
    );
    const navigate = useNavigate();
    const batchRef = useRef();

    // загрузка списка помещений
    useEffect(() => {
        apiClient.get('/api/Room').then(r => setRooms(r.data));
    }, []);

    const filtered = rooms.filter(rm =>
        rm.name.toLowerCase().includes(q.toLowerCase()) ||
        rm.floor?.name.toLowerCase().includes(q.toLowerCase())
    );

    // генерировать PDF с QR для всех помещений
    const generateAllQR = async () => {
        if (!rooms.length) {
            alert('Список помещений пуст');
            return;
        }

        // ждём рендер скрытых .qr-page
        await new Promise(res => setTimeout(res, 100));

        const pdf = new jsPDF('p', 'pt', 'a4');
        const pages = batchRef.current.children;

        for (let i = 0; i < pages.length; i++) {
            const el = pages[i];
            const canvas = await html2canvas(el, { scale: 2 });
            const img = canvas.toDataURL('image/png');
            const w = pdf.internal.pageSize.getWidth();
            const h = (canvas.height * w) / canvas.width;
            if (i > 0) pdf.addPage();
            pdf.addImage(img, 'PNG', 0, 0, w, h);
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
                        <a href="/accessmatrix" className="active">Матрица доступа</a>
                        <a href="/dashboard">Лог событий</a>
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
                                <tr>
                                    <th>ID</th>
                                    <th>Номер помещения</th>
                                    <th>Этаж</th>
                                </tr>
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

            {/* Скрытый контейнер с макетами страниц для PDF */}
            <div className="QRBatchContainer" ref={batchRef}>
                {rooms.map(rm => {
                    const expires = new Date(Date.now() + 1.5 * 3600 * 1000);
                    const hh = expires.getHours().toString().padStart(2, '0');
                    const mm = expires.getMinutes().toString().padStart(2, '0');
                    return (
                        <div key={rm.id} className="qr-page">
                            <h2 className="qr-header">QR-код для всех помещений</h2>
                            <div className="qr-wrapper">
                                <QRCode value={JSON.stringify({ roomId: rm.id })} size={200} />
                            </div>
                            <div className="qr-info">
                                <div className="room-label">Помещение №{rm.name}</div>
                                <div className="lifetime">Время работы: {hh}:{mm}</div>
                            </div>
                        </div>
                    );
                })}
            </div>
        </div>
    );
}

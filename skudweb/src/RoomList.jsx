import React, { useEffect, useState } from 'react';
import { NavLink, Link, useNavigate } from 'react-router-dom';
import apiClient from './apiClient';
import logo from './assets/natk-logo.png';
import './ListPage.css';

export default function RoomList() {
    const [rooms, setRooms] = useState([]);
    const [q, setQ] = useState('');
    const [collapsed, setCollapsed] = useState(
        () => JSON.parse(localStorage.getItem('sidebar-collapsed') || 'false')
    );
    const navigate = useNavigate();

    useEffect(() => {
        apiClient.get('/api/Room').then(r => setRooms(r.data));
    }, []);

    const filtered = rooms.filter(rm =>
        rm.name.toLowerCase().includes(q.toLowerCase()) ||
        rm.floor?.name.toLowerCase().includes(q.toLowerCase())
    );

    const toggleSidebar = () => {
        const nc = !collapsed;
        setCollapsed(nc);
        localStorage.setItem('sidebar-collapsed', JSON.stringify(nc));
    };

    const handleRowClick = (id) => {
        navigate(`/rooms/${id}`);
    };

    return (
        <div className="Dashboard">
            <header className="Header">
                <button className="Burger SidebarBurger" onClick={toggleSidebar} aria-label="Toggle sidebar">
                    <span /><span /><span />
                </button>
                <img src={logo} alt="НАТК" className="Header-logo" />
                <span className="Header-title">Помещения</span>
            </header>

            <div className="Body">
                <aside className={`Sidebar ${collapsed ? 'collapsed' : ''}`}>
                    <nav>
                        <NavLink to="/employees">Сотрудники</NavLink>
                        <NavLink to="/devices">Устройства</NavLink>
                        <NavLink to="/accessmatrix">Матрица доступа</NavLink>
                        <NavLink to="/logs">Лог событий</NavLink>
                        <NavLink to="/reports">Отчёты</NavLink>
                        <NavLink to="/settings">Настройки</NavLink>
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
                                        onClick={() => handleRowClick(rm.id)}
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
                            Добавление помещения
                        </Link>
                    </div>
                </main>
            </div>
        </div>
    );
}

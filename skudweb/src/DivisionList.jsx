﻿import React, { useEffect, useState } from 'react';
import { NavLink, Link } from 'react-router-dom';
import apiClient from './apiClient';
import logo from './assets/natk-logo.png';
import './ListPage.css';

export default function DivisionList() {
    const [divisions, setDivisions] = useState([]);
    const [q, setQ] = useState('');
    const [collapsed, setCollapsed] = useState(
        () => JSON.parse(localStorage.getItem('sidebar-collapsed') || 'false')
    );

    useEffect(() => {
        apiClient.get('/api/Division').then(r => setDivisions(r.data));
    }, []);

    const filtered = divisions.filter(d =>
        d.name.toLowerCase().includes(q.toLowerCase())
    );

    const toggleSidebar = () => {
        const nc = !collapsed;
        setCollapsed(nc);
        localStorage.setItem('sidebar-collapsed', JSON.stringify(nc));
    };

    return (
        <div className="Dashboard">
            <header className="Header">
                <button className="Burger SidebarBurger" onClick={toggleSidebar} aria-label="Toggle sidebar">
                    <span /><span /><span />
                </button>
                <img src={logo} alt="НАТК" className="Header-logo" />
                <span className="Header-title">Подразделения</span>
            </header>

            <div className="Body">
                <aside className={`Sidebar ${collapsed ? 'collapsed' : ''}`}>
                    <nav>
                        <a href="/employees">Сотрудники</a>
                        <a href="/devices">Устройства</a>
                        <a href="/accessmatrix" >Матрица доступа</a>
                        <a href="/dashboard">Лог событий</a>
                        <a href="/rooms">Помещения</a>
                        <a href="/divisions" className="active">Подразделения</a>
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
                    </div>

                    <div className="TableWrapper">
                        <table className="ListTable">
                            <thead>
                                <tr>
                                    <th>ID</th>
                                    <th>Название подразделения</th>
                                </tr>
                            </thead>
                            <tbody>
                                {filtered.map(d => (
                                    <tr key={d.id}>
                                        <td>{d.id}</td>
                                        <td>{d.name}</td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    </div>

                    <div className="Actions">
                        <Link to="/divisions/new" className="AddBtn">
                            Добавление Подразделения
                        </Link>
                    </div>
                </main>
            </div>
        </div>
    );
}

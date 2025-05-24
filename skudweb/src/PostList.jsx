import React, { useEffect, useState } from 'react';
import { NavLink, Link } from 'react-router-dom';
import apiClient from './apiClient';
import logo from './assets/natk-logo.png';
import './ListPage.css';

export default function PostList() {
    const [posts, setPosts] = useState([]);
    const [q, setQ] = useState('');
    const [collapsed, setCollapsed] = useState(
        () => JSON.parse(localStorage.getItem('sidebar-collapsed') || 'false')
    );

    useEffect(() => {
        apiClient.get('/api/Post').then(r => setPosts(r.data));
    }, []);

    const filtered = posts.filter(p =>
        p.name.toLowerCase().includes(q.toLowerCase()) ||
        p.division?.name.toLowerCase().includes(q.toLowerCase())
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
                <span className="Header-title">Должности</span>
            </header>

            <div className="Body">
                <aside className={`Sidebar ${collapsed ? 'collapsed' : ''}`}>
                    <nav>
                        <a href="/employees">Сотрудники</a>
                        <a href="/devices">Устройства</a>
                        <a href="/accessmatrix" >Матрица доступа</a>
                        <a href="/dashboard">Лог событий</a>
                        <a href="/rooms">Помещения</a>
                        <a href="/divisions">Подразделения</a>
                        <a href="/posts" className="active">Должности</a>
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
                                    <th>Название должности</th>
                                    <th>Подразделение</th>
                                </tr>
                            </thead>
                            <tbody>
                                {filtered.map(p => (
                                    <tr key={p.id}>
                                        <td>{p.id}</td>
                                        <td>{p.name}</td>
                                        <td>{p.division?.name}</td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    </div>

                    <div className="Actions">
                        <Link to="/posts/new" className="AddBtn">
                            Добавление Должности
                        </Link>
                    </div>
                </main>
            </div>
        </div>
    );
}

import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import logo from './assets/natk-logo.png';
import {
    fetchFloors,
    fetchDivisions,
    fetchRoomsAll,
    fetchRoomsByFloor,
    fetchAccessMatrix
} from './accessMatrixApi';
import './AccessMatrixPage.css';

export default function AccessMatrixPage() {
    const [collapsed, setCollapsed] = useState(
        () => JSON.parse(localStorage.getItem('sidebar-collapsed') || 'false')
    );
    const [floors, setFloors] = useState([]);
    const [divisions, setDivisions] = useState([]);
    const [rooms, setRooms] = useState([]);
    const [matrix, setMatrix] = useState([]);

    const [floorId, setFloorId] = useState(null);
    const [divisionId, setDivisionId] = useState(null);

    useEffect(() => {
        fetchFloors().then(setFloors);
        fetchDivisions().then(setDivisions);
        // сразу загрузим все комнаты, пока нет выбранного этажа
        fetchRoomsAll().then(setRooms);
    }, []);

    useEffect(() => {
        if (floorId == null) {
            fetchRoomsAll().then(setRooms);
        } else {
            fetchRoomsByFloor(floorId).then(setRooms);
        }
    }, [floorId]);

    useEffect(() => {
        fetchAccessMatrix({ floorId, divisionId })
            .then(setMatrix);
    }, [floorId, divisionId]);

    const toggleSidebar = () => {
        const nc = !collapsed;
        setCollapsed(nc);
        localStorage.setItem('sidebar-collapsed', JSON.stringify(nc));
    };

    const posts = Array.from(
        new Map(matrix.map(m => [m.post.id, m.post])).values()
    );

    return (
        <div className="Dashboard">
            <header className="Header">
                <img src={logo} alt="НАТК" className="Header-logo" />
                <span className="Header-title">НАТК</span>
            </header>

            <div className="Body">
                <aside className={`Sidebar ${collapsed ? 'collapsed' : ''}`}>
                    <button
                        className="Burger SidebarBurger"
                        onClick={() => setCollapsed(c => !c)}
                        aria-label="Toggle sidebar"
                    >
                        <span /><span /><span />
                    </button>
                    <nav>
                        <a href="/employees">Сотрудники</a>
                        <a href="/devices">Устройства</a>
                        <a href="/accessmatrix" className="active"> Матрица доступа</a>
                        <a href="/dashboard">Лог событий</a>
                        <a href="/reports">Отчёты</a>
                        <a href="/setting">Настройки</a>
                    </nav>
                </aside>

                <main className="Main">
                    <div className="MatrixFilters">
                        <label>
                            Этаж:
                            <select
                                value={floorId || ''}
                                onChange={e => setFloorId(e.target.value ? +e.target.value : null)}
                            >
                                <option value="">— все —</option>
                                {floors.map(f =>
                                    <option key={f.id} value={f.id}>{f.name}</option>
                                )}
                            </select>
                        </label>

                        <label>
                            Подразделение:
                            <select
                                value={divisionId || ''}
                                onChange={e => setDivisionId(e.target.value ? +e.target.value : null)}
                            >
                                <option value="">— все —</option>
                                {divisions.map(d =>
                                    <option key={d.id} value={d.id}>{d.name}</option>
                                )}
                            </select>
                        </label>
                    </div>

                    <div className="TableWrapper">
                        <table className="MatrixTable">
                            <thead>
                                <tr>
                                    <th>Подразделение</th>
                                    <th>Должность</th>
                                    {rooms.map(r => <th key={r.id}>{r.name}</th>)}
                                </tr>
                            </thead>
                            <tbody>
                                {posts.map(post => (
                                    <tr key={post.id}>
                                        <td>{post.division.name}</td>
                                        <td>{post.name}</td>
                                        {rooms.map(room => {
                                            const rec = matrix.find(m =>
                                                m.post.id === post.id && m.room.id === room.id
                                            );
                                            const ok = rec?.isAccess;
                                            return (
                                                <td
                                                    key={room.id}
                                                    className={ok ? 'MatrixCell ok' : 'MatrixCell no'}
                                                >
                                                    {ok ? '✔' : '✖'}
                                                </td>
                                            );
                                        })}
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    </div>

                    <div className="MatrixActions">
                        <button className="Btn">Управление должностями</button>
                        <button className="Btn">Управление помещениями</button>
                        <button className="Btn">Сгенерировать QR</button>
                    </div>
                </main>
            </div>
        </div>
    );
}

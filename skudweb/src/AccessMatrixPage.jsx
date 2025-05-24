import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import Swal from 'sweetalert2';
import logo from './assets/natk-logo.png';
import {
    fetchFloors,
    fetchDivisions,
    fetchRoomsAll,
    fetchAccessMatrix,
    toggleAccessMatrixEntry
} from './accessMatrixApi';
import './AccessMatrixPage.css';

export default function AccessMatrixPage() {
    const [collapsed, setCollapsed] = useState(false);
    const [floors, setFloors] = useState([]);
    const [divisions, setDivisions] = useState([]);
    const [rooms, setRooms] = useState([]);
    const [matrix, setMatrix] = useState([]);

    const [floorId, setFloorId] = useState(null);
    const [divisionId, setDivisionId] = useState(null);

    useEffect(() => {
        fetchFloors().then(setFloors);
        fetchDivisions().then(setDivisions);
    }, []);

    useEffect(() => {
        fetchRoomsAll().then(allRooms => {
            setRooms(floorId ? allRooms.filter(r => r.floor?.id === floorId) : allRooms);
        });
    }, [floorId]);

    useEffect(() => {
        fetchAccessMatrix({ floorId, divisionId }).then(setMatrix);
    }, [floorId, divisionId]);

    const posts = Array.from(new Map(matrix.map(x => [x.post.id, x.post])).values());

    const toggleAccess = async (entry) => {
        try {
            await toggleAccessMatrixEntry(entry.id);
            Swal.fire({
                icon: 'success',
                title: `Доступ изменён`,
                toast: true,
                position: 'top-end',
                timer: 1200,
                showConfirmButton: false
            });
            const updated = await fetchAccessMatrix({ floorId, divisionId });
            setMatrix(updated);
        } catch (err) {
            console.error('Ошибка при обновлении доступа:', err);
            Swal.fire({
                icon: 'error',
                title: 'Ошибка изменения доступа',
                text: err.response?.data?.message || 'Не удалось изменить доступ'
            });
        }
    };

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
                        <a href="/accessmatrix" className="active">Матрица доступа</a>
                        <a href="/dashboard">Лог событий</a>
                        <a href="/rooms">Помещения</a>
                        <a href="/divisions">Подразделения</a>
                        <a href="/posts">Должности</a>
                        <a href="/reports">Отчёты</a>
                        <a href="/setting">Настройки</a>
                    </nav>
                </aside>


                <main className="Main">
                    <div className="MatrixFilters">
                        <label>
                            Этаж:
                            <select
                                value={floorId ?? ''}
                                onChange={e => setFloorId(e.target.value ? Number(e.target.value) : null)}
                            >
                                <option value="">— все —</option>
                                {floors.map(f => (
                                    <option key={f.id} value={f.id}>{f.name}</option>
                                ))}
                            </select>
                        </label>

                        <label>
                            Подразделение:
                            <select
                                value={divisionId ?? ''}
                                onChange={e => setDivisionId(e.target.value ? Number(e.target.value) : null)}
                            >
                                <option value="">— все —</option>
                                {divisions.map(d => (
                                    <option key={d.id} value={d.id}>{d.name}</option>
                                ))}
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
                                            const entry = matrix.find(
                                                m => m.post.id === post.id && m.room.id === room.id
                                            );

                                            if (!entry) {
                                                return (
                                                    <td key={room.id} className="MatrixCell empty">—</td>
                                                );
                                            }

                                            return (
                                                <td
                                                    key={room.id}
                                                    className={entry.isAccess ? 'MatrixCell ok' : 'MatrixCell no'}
                                                    onClick={() => toggleAccess(entry)}
                                                    style={{ cursor: 'pointer' }}
                                                    title="Клик для изменения доступа"
                                                >
                                                    {entry.isAccess ? '✔' : '✖'}
                                                </td>
                                            );
                                        })}
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    </div>

                    <div className="MatrixActions">
                        <Link to="/accessmatrix/new" className="Btn">Добавить запись</Link>
                    </div>

                    <div className="MatrixButtons">
                        <Link to="/posts" className="Btn blue">Управление должностями</Link>
                        <Link to="/divisions" className="Btn blue">Управление подразделениями</Link>
                        <Link to="/rooms" className="Btn blue">Управление помещениями</Link>
                    </div>
                </main>
            </div>
        </div>
    );
}

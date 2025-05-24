import React, { useEffect, useState } from 'react';
import { fetchStats, fetchAttempts } from './dashboardApi';
import logo from './assets/natk-logo.png';
import './Dashboard.css';

export default function Dashboard() {
    const [counts, setCounts] = useState({ employees: 0, devices: 0, attempts: 0 });
    const [attempts, setAttempts] = useState([]);
    const [collapsed, setCollapsed] = useState(() => {
        const saved = localStorage.getItem('sidebar-collapsed');
        return saved ? JSON.parse(saved) : false;
    });

    // фильтры — локальные поля формы
    const [filterInputs, setFilterInputs] = useState({
        from: '', to: '', pointId: '', employeeId: ''
    });
    // собственно фильтры, по которым делаем запрос
    const [filters, setFilters] = useState({ take: 10 });

    // сохранить состояние сайдбара
    useEffect(() => {
        localStorage.setItem('sidebar-collapsed', JSON.stringify(collapsed));
    }, [collapsed]);

    // при изменении filters — заново фетчим данные
    useEffect(() => {
        // stats
        fetchStats().then(dto => {
            setCounts(c => ({
                ...c,
                employees: dto.employeesCount,
                devices: dto.devicesCount
            }));
        });
        // attempts
        fetchAttempts(filters).then(arr => {
            setAttempts(arr);
            setCounts(c => ({ ...c, attempts: arr.length }));
        });
    }, [filters]);

    function onApply() {
        setFilters({
            from: filterInputs.from || undefined,
            to: filterInputs.to || undefined,
            pointId: filterInputs.pointId ? Number(filterInputs.pointId) : undefined,
            employeeId: filterInputs.employeeId ? Number(filterInputs.employeeId) : undefined,
            take: 10
        });
    }
    function onReset() {
        setFilterInputs({ from: '', to: '', pointId: '', employeeId: '' });
        setFilters({ take: 10 });
    }

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
                        <a href="/accessmatrix" >Матрица доступа</a>
                        <a href="/dashboard" className="active">Лог событий</a>
                        <a href="/rooms">Помещения</a>
                        <a href="/divisions">Подразделения</a>
                        <a href="/posts">Должности</a>
                        <a href="/reports">Отчёты</a>
                        <a href="/setting">Настройки</a>
                    </nav>
                </aside>

                <main className="Main">
                    <section className="Stats">
                        <div className="Card">Сотрудников: {counts.employees}</div>
                        <div className="Card">Устройств: {counts.devices}</div>
                        <div className="Card">Попыток: {counts.attempts}</div>
                    </section>

                    <section className="Filters">
                        <label>
                            From: <input
                                type="date"
                                name="from"
                                value={filterInputs.from}
                                onChange={e => setFilterInputs(fi => ({ ...fi, from: e.target.value }))}
                            />
                        </label>{' '}
                        <label>
                            To: <input
                                type="date"
                                name="to"
                                value={filterInputs.to}
                                onChange={e => setFilterInputs(fi => ({ ...fi, to: e.target.value }))}
                            />
                        </label>{' '}
                        <label>
                            Point ID: <input
                                type="number"
                                name="pointId"
                                value={filterInputs.pointId}
                                onChange={e => setFilterInputs(fi => ({ ...fi, pointId: e.target.value }))}
                                style={{ width: '4rem' }}
                            />
                        </label>{' '}
                        <label>
                            Emp ID: <input
                                type="number"
                                name="employeeId"
                                value={filterInputs.employeeId}
                                onChange={e => setFilterInputs(fi => ({ ...fi, employeeId: e.target.value }))}
                                style={{ width: '4rem' }}
                            />
                        </label>{' '}
                        <button onClick={onApply}>Применить</button>{' '}
                        <button onClick={onReset}>Сбросить</button>
                    </section>

                    <section className="TableWrapper">
                        <table className="AttemptsTable">
                            <thead>
                                <tr>
                                    <th>Дата/время</th>
                                    <th>Сотрудник</th>
                                    <th>Точка</th>
                                    <th>IP-адрес</th>
                                    <th>Результат</th>
                                </tr>
                            </thead>
                            <tbody>
                                {attempts.map((a, i) => (
                                    <tr key={i}>
                                        <td>{new Date(a.timestamp).toLocaleString()}</td>
                                        <td>{a.employeeFullName}</td>
                                        <td>{a.pointName}</td>
                                        <td>{a.ipAddress}</td>
                                        <td className={a.success ? 'Success' : 'Fail'}>
                                            {a.success ? 'Успех' : 'Провал'}
                                        </td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    </section>
                </main>
            </div>
        </div>
    );
}

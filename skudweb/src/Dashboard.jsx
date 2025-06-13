import React, { useEffect, useState, useRef } from 'react';
import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import logo from './assets/natk-logo.png';
import './Dashboard.css';

// Utility function to format IP address
function formatIpAddress(ipAddress) {
    if (ipAddress === '::1') {
        return '127.0.0.1';
    }
    return ipAddress.replace(/^::ffff:/, '');
}

// Full API URLs
const API_BASE_URL = 'https://localhost:7267/api/Dashboard';
const SIGNALR_HUB_URL = 'https://localhost:7267/hubs/logs';

// Fetch functions with full API address
async function fetchStats() {
    const resp = await fetch(`${API_BASE_URL}/stats`);
    if (!resp.ok) throw new Error('Ошибка получения статистики');
    return resp.json();
}

async function fetchAttempts(params = {}) {
    const q = new URLSearchParams();
    if (params.from) q.append('from', params.from);
    if (params.to) q.append('to', params.to);
    if (params.pointId) q.append('pointId', String(params.pointId));
    if (params.employeeId) q.append('employeeId', String(params.employeeId));
    if (params.page) q.append('page', String(params.page));
    q.append('take', String(params.take ?? 10));

    const resp = await fetch(`${API_BASE_URL}/attempts?${q.toString()}`);
    if (!resp.ok) throw new Error('Ошибка получения попыток');
    return resp.json();
}

export default function Dashboard() {
    const [counts, setCounts] = useState({ employees: 0, devices: 0, attempts: 0 });
    const [attempts, setAttempts] = useState([]);
    const [collapsed, setCollapsed] = useState(() => {
        const saved = localStorage.getItem('sidebar-collapsed');
        return saved ? JSON.parse(saved) : false;
    });

    const [filterInputs, setFilterInputs] = useState({
        from: '',
        to: '',
        pointId: '',
        employeeId: ''
    });

    const [filters, setFilters] = useState({ take: 10, page: 1 });
    const [totalPages, setTotalPages] = useState(1);

    const connection = useRef(null);

    useEffect(() => {
        localStorage.setItem('sidebar-collapsed', JSON.stringify(collapsed));
    }, [collapsed]);

    useEffect(() => {
        fetchStats().then(dto => {
            setCounts(c => ({
                ...c,
                employees: dto.employeesCount,
                devices: dto.devicesCount
            })).catch(console.error);
        });

        fetchAttempts(filters).then(data => {
            setAttempts(data.attempts);
            setCounts(c => ({ ...c, attempts: data.totalCount }));
            setTotalPages(Math.ceil(data.totalCount / filters.take));
        }).catch(console.error);
    }, [filters]);

    useEffect(() => {
        const conn = new HubConnectionBuilder()
            .withUrl(SIGNALR_HUB_URL)
            .configureLogging(LogLevel.Information)
            .withAutomaticReconnect()
            .build();

        conn.start()
            .then(() => console.log('SignalR Connected'))
            .catch(e => console.error('SignalR Connection Error:', e));

        conn.on('ReceiveLog', (log) => {
            setAttempts(prev => {
                const newList = [log, ...prev];
                return newList.slice(0, 50);
            });
            setCounts(c => ({ ...c, attempts: c.attempts + 1 }));
        });

        connection.current = conn;

        return () => {
            if (connection.current) {
                connection.current.stop();
            }
        };
    }, []);

    function onApply() {
        setFilters({
            from: filterInputs.from || undefined,
            to: filterInputs.to || undefined,
            pointId: filterInputs.pointId ? Number(filterInputs.pointId) : undefined,
            employeeId: filterInputs.employeeId ? Number(filterInputs.employeeId) : undefined,
            take: 10,
            page: 1
        });
    }

    function onReset() {
        setFilterInputs({ from: '', to: '', pointId: '', employeeId: '' });
        setFilters({ take: 10, page: 1 });
    }

    function goToPage(page) {
        setFilters({ ...filters, page });
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
                        <a href="/accessmatrix">Матрица доступа</a>
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
                                    <th>IP-адрес</th>
                                    <th>Результат</th>
                                </tr>
                            </thead>
                            <tbody>
                                {attempts.map((a, i) => (
                                    <tr key={i}>
                                        <td>{new Date(a.timestamp).toLocaleString()}</td>
                                        <td>{a.employeeFullName}</td>
                                        <td>{formatIpAddress(a.ipAddress)}</td>
                                        <td className={a.success ? 'Success' : 'Fail'}>
                                            {a.success ? 'Успех' : 'Провал'}
                                        </td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    </section>

                    <section className="Pagination">
                        <button
                            className="PaginationButton"
                            onClick={() => goToPage(filters.page - 1)}
                            disabled={filters.page <= 1}
                        >
                            Предыдущая
                        </button>
                        <span className="PageInfo">Страница {filters.page} из {totalPages}</span>
                        <button
                            className="PaginationButton"
                            onClick={() => goToPage(filters.page + 1)}
                            disabled={filters.page >= totalPages}
                        >
                            Следующая
                        </button>
                    </section>
                </main>
            </div>
        </div>
    );
}

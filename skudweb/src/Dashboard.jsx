import React, { useEffect, useState } from 'react'
import { fetchRecentAttempts } from './dashboardApi'
// Импортируем логотип из папки assets:
import logo from './assets/natk-logo.png'
import './Dashboard.css'

export default function Dashboard() {
    const [attempts, setAttempts] = useState([])
    const [counts, setCounts] = useState({ employees: 0, devices: 0, attempts: 0 })
    const [collapsed, setCollapsed] = useState(() => {
        const saved = localStorage.getItem('sidebar-collapsed')
        return saved ? JSON.parse(saved) : false
    })

    useEffect(() => {
        localStorage.setItem('sidebar-collapsed', JSON.stringify(collapsed))
    }, [collapsed])

    useEffect(() => {
        fetchRecentAttempts(10).then(data => {
            setAttempts(data)
            setCounts(c => ({ ...c, attempts: data.length }))
        })
        setCounts(c => ({ ...c, employees: 128, devices: 43 }))
    }, [])

    return (
        <div className="Dashboard">
            <header className="Header">
                {/* Логотип */}
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
                        <a href="#">Сотрудники</a>
                        <a href="#">Устройства</a>
                        <a href="#">Пропуски</a>
                        <a href="#">Лог событий</a>
                        <a href="#">Отчёты</a>
                        <a href="#">Настройки</a>
                    </nav>
                </aside>

                <main className="Main">
                    <section className="Stats">
                        <div className="Card">Сотрудников: {counts.employees}</div>
                        <div className="Card">Устройств: {counts.devices}</div>
                        <div className="Card">Попыток: {counts.attempts}</div>
                    </section>

                    <section className="Filters">
                        Фильтры: Дата, точка прохода, Сотрудник <button>Применить</button>{' '}
                        <button>Сбросить</button>
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
    )
}

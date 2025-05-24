import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { fetchEmployees, getAvatarUrl } from './employeeApi';
import logo from './assets/natk-logo.png';
import './Employee.css';

export default function EmployeeList() {
    const [collapsed, setCollapsed] = useState(
        () => JSON.parse(localStorage.getItem('sidebar-collapsed') || 'false')
    );
    const [inputs, setInputs] = useState({ fullName: '', phone: '', login: '' });
    const [filters, setFilters] = useState({ take: 100 });
    const [employees, setEmployees] = useState([]);

    useEffect(() => {
        localStorage.setItem('sidebar-collapsed', JSON.stringify(collapsed));
    }, [collapsed]);

    useEffect(() => {
        fetchEmployees(filters)
            .then(setEmployees)
            .catch(console.error);
    }, [filters]);

    const handleInput = e => {
        const { name, value } = e.target;
        setInputs(i => ({ ...i, [name]: value }));
    };

    const applyFilters = () => {
        setFilters({
            fullName: inputs.fullName || undefined,
            phone: inputs.phone || undefined,
            login: inputs.login || undefined,
            take: 100
        });
    };

    const resetFilters = () => {
        setInputs({ fullName: '', phone: '', login: '' });
        setFilters({ take: 100 });
    };

    return (
        <div className="Dashboard">
            <header className="Header">
                <img src={logo} alt="НАТК" className="Header-logo" />
                <span className="Header-title">НАТК</span>
                <Link to="/employees/new" className="AddButton">Добавить сотрудника</Link>
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
                        <a href="/employees" className="active">Сотрудники</a>
                        <a href="/devices">Устройства</a>
                        <a href="/accessmatrix" >Матрица доступа</a>
                        <a href="/dashboard">Лог событий</a>
                        <a href="/rooms">Помещения</a>
                        <a href="/divisions">Подразделения</a>
                        <a href="/posts">Должности</a>
                        <a href="/reports">Отчёты</a>
                        <a href="/setting">Настройки</a>
                    </nav>
                </aside>
   

                <main className="Main">
                    <section className="Filters">
                        <label>
                            ФИО:
                            <input
                                name="fullName"
                                value={inputs.fullName}
                                onChange={handleInput}
                                placeholder="Введите ФИО"
                            />
                        </label>
                        <label>
                            Телефон:
                            <input
                                name="phone"
                                value={inputs.phone}
                                onChange={handleInput}
                                placeholder="Введите телефон"
                            />
                        </label>
                        <label>
                            Логин:
                            <input
                                name="login"
                                value={inputs.login}
                                onChange={handleInput}
                                placeholder="Введите логин"
                            />
                        </label>
                        <button onClick={applyFilters}>Применить</button>
                        <button onClick={resetFilters}>Сбросить</button>
                    </section>

                    <section className="TableWrapper">
                        <table className="EmployeesTable">
                            <thead>
                                <tr>
                                    <th>Фото</th>
                                    <th>Сотрудник</th>
                                    <th>Телефон</th>
                                    <th>Логин</th>
                                    <th>Подразделение</th>
                                    <th>Должность</th>
                                </tr>
                            </thead>
                            <tbody>
                                {employees.map(emp => (
                                    <tr key={emp.id}>
                                        <td className="AvatarCell">
                                            {emp.avatar
                                                ? <img
                                                    src={getAvatarUrl(emp.id) + '?t=' + Date.now()}
                                                    alt="avatar"
                                                    className="TableAvatar"
                                                />
                                                : <div className="TableAvatarPlaceholder" />
                                            }
                                        </td>
                                        <td>
                                            <Link to={`/employees/${emp.id}`}>
                                                {emp.lastName} {emp.firstName[0]}. {emp.patronymic?.[0] || ''}.
                                            </Link>
                                        </td>
                                        <td>{emp.phoneNumber}</td>
                                        <td>{emp.login}</td>
                                        <td>{emp.division?.name}</td>
                                        <td>{emp.post?.name}</td>
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

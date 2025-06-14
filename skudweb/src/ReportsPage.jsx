import React, { useState, useEffect } from 'react';
import { generateReport } from './reportsApi';
import logo from './assets/natk-logo.png';
import './ReportsPage.css';
import docIcon from './assets/doc_icon.png';
import pdfIcon from './assets/pdf_icon.png';

export default function ReportsPage() {
    const [collapsed, setCollapsed] = useState(() => {
        const saved = localStorage.getItem('sidebar-collapsed');
        return saved ? JSON.parse(saved) : false;
    });

    const [options, setOptions] = useState({
        includeAccessAttempts: true,
        successOnly: false,
        failedOnly: false,
        fromDate: '',
        toDate: '',
    });

    useEffect(() => {
        localStorage.setItem('sidebar-collapsed', JSON.stringify(collapsed));
    }, [collapsed]);

    const onChange = e => {
        const { name, checked, value } = e.target;
        if (name === "fromDate" || name === "toDate") {
            setOptions(prev => ({ ...prev, [name]: value }));
        } else {
            setOptions(prev => ({ ...prev, [name]: checked }));
        }
    };

    const onGenerate = async format => {
        try {
            const blob = await generateReport(format, options);
            const link = document.createElement('a');
            link.href = window.URL.createObjectURL(blob);
            link.download = `report.${format}`;
            link.click();
        } catch (err) {
            console.error(err);
            alert('Не удалось скачать отчёт');
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
                        <a href="/accessmatrix" >Матрица доступа</a>
                        <a href="/dashboard">Лог событий</a>
                        <a href="/rooms">Помещения</a>
                        <a href="/divisions">Подразделения</a>
                        <a href="/posts">Должности</a>
                        <a href="/reports" className="active">Отчёты</a>
                        <a href="/setting">Настройки</a>
                    </nav>
                </aside>

                <main className="Main">
                    <section className="ReportsContainer">
                        <h1>Генерация отчётов</h1>
                        <p className="Subtitle">Выберите параметры для отчёта</p>

                        <div className="CheckboxGroup">
                            <label>
                                <input
                                    type="checkbox"
                                    name="includeAccessAttempts"
                                    checked={options.includeAccessAttempts}
                                    onChange={onChange}
                                /> Попытки доступа
                            </label>
                            <label>
                                <input
                                    type="checkbox"
                                    name="successOnly"
                                    checked={options.successOnly}
                                    onChange={onChange}
                                /> Удачные попытки
                            </label>
                            <label>
                                <input
                                    type="checkbox"
                                    name="failedOnly"
                                    checked={options.failedOnly}
                                    onChange={onChange}
                                /> Неудачные попытки
                            </label>
                        </div>

                        <div className="DateRangeGroup">
                            <label>
                                От:
                                <input
                                    type="date"
                                    name="fromDate"
                                    value={options.fromDate}
                                    onChange={onChange}
                                />
                            </label>
                            <label>
                                До:
                                <input
                                    type="date"
                                    name="toDate"
                                    value={options.toDate}
                                    onChange={onChange}
                                />
                            </label>
                        </div>

                        <div className="FormatOptions">
                            <div className="FormatCard" onClick={() => onGenerate('docx')}>
                                <img src={docIcon} alt="DOCX" className="FormatImage" />
                                <div className="FormatLabel">DOCX</div>
                            </div>
                            <div className="FormatCard" onClick={() => onGenerate('pdf')}>
                                <img src={pdfIcon} alt="PDF" className="FormatImage" />
                                <div className="FormatLabel">PDF</div>
                            </div>
                        </div>
                    </section>
                </main>
            </div>
        </div>
    );
}

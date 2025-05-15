// src/NotFound.jsx
import React from 'react';
import { useNavigate } from 'react-router-dom';
import './NotFound.css';
import logo from './assets/natk-logo.png'; 

export default function NotFound() {
    const nav = useNavigate();
    return (
        <div className="NotFoundPage">
            <header className="Header">
                <img src={logo} alt="НАТК" className="Header-logo" />
                <span className="Header-title">НАТК</span>
            </header>
            <main className="NF-Body">
                <div className="NF-Content">
                    <div className="NF-Icon">☹️</div>
                    <h1>404</h1>
                    <p>Что-то пошло не так</p>
                    <button
                        className="NF-Button"
                        onClick={() => nav(-1)}
                    >
                        Вернуться на предыдущую страницу
                    </button>
                </div>
            </main>
        </div>
    );
}

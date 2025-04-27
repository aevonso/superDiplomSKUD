// src/App.jsx
import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import logo from './assets/natk-logo.png';
import apiClient from './apiClient';
import './App.css';

export default function App() {
    const [login, setLogin] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');
    const navigate = useNavigate();

    const handleSubmit = async e => {
        e.preventDefault();
        setError('');
        try {
            const { data } = await apiClient.post('/auth/login', { login, password });
            // сохраняем токены и логин
            localStorage.setItem('accessToken', data.accessToken);
            localStorage.setItem('refreshToken', data.refreshToken);
            localStorage.setItem('login', login);
            // переходим на страницу 2FA
            navigate('/2fa');
        } catch (err) {
            if (err.response?.status === 401) setError('Неверный логин или пароль');
            else setError('Ошибка соединения с сервером');
        }
    };

    return (
        <div className="App">
            <header className="App-header">
                <img src={logo} alt="НАТК" className="App-logo" />
            </header>
            <main className="App-main">
                <form className="LoginForm" onSubmit={handleSubmit}>
                    <h1>Авторизация в систему</h1>
                    {error && <p className="Error">{error}</p>}
                    <div className="FormGroup">
                        <label htmlFor="login">Логин</label>
                        <input
                            id="login"
                            value={login}
                            onChange={e => setLogin(e.target.value)}
                            placeholder="Введите логин"
                        />
                    </div>
                    <div className="FormGroup">
                        <label htmlFor="password">Пароль</label>
                        <input
                            id="password"
                            type="password"
                            value={password}
                            onChange={e => setPassword(e.target.value)}
                            placeholder="Введите пароль"
                        />
                    </div>
                    <button type="submit" className="SubmitButton">Войти</button>
                </form>
            </main>
        </div>
    );
}

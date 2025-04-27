// src/TwoFactor.jsx
import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import apiClient from './apiClient';
import logo from './assets/natk-logo.png';
import './TwoFactor.css';

export default function TwoFactor() {
    const [code, setCode] = useState('');
    const [error, setError] = useState('');
    const [loading, setLoading] = useState(false);
    const navigate = useNavigate();
    const login = localStorage.getItem('login');

    // при монтировании отправляем generate
    useEffect(() => {
        if (login) {
            apiClient.post('/auth/2fa/generate', { login })
                .catch(() => setError('Не удалось отправить код на почту'));
        } else {
            navigate('/'); // если нет login — обратно на логин
        }
    }, []);

    const handleSubmit = async e => {
        e.preventDefault();
        if (!code) {
            setError('Пожалуйста, введите код');
            return;
        }
        setLoading(true);
        try {
            const { data } = await apiClient.post('/auth/2fa/validate', { login, code });
            if (data.success) navigate('/dashboard');
            else setError(data.message || 'Неверный код');
        } catch {
            setError('Ошибка соединения с сервером');
        } finally {
            setLoading(false);
        }
    };

    const monkeyEmoji = code ? '🙈' : '🙉';

    return (
        <div className="TwoFactor">
            <header className="App-header">
                <img src={logo} alt="НАТК" className="App-logo" />
            </header>
            <main className="TwoFactor-main">
                <form className="TwoFactor-form" onSubmit={handleSubmit}>
                    <div className="TwoFactor-monkey">{monkeyEmoji}</div>
                    <label htmlFor="code" className="TwoFactor-label">
                        Введите 6-значный код из письма
                    </label>
                    <input
                        id="code"
                        className="TwoFactor-input"
                        type="text"
                        value={code}
                        onChange={e => { setCode(e.target.value.trim()); setError(''); }}
                        disabled={loading}
                    />
                    {error && <div className="TwoFactor-error">{error}</div>}
                    <button type="submit" className="TwoFactor-button" disabled={loading}>
                        {loading ? 'Проверка…' : 'Подтвердить'}
                    </button>
                </form>
            </main>
        </div>
    );
}

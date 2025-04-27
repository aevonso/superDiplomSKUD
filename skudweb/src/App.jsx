import React from 'react'
import { Routes, Route, Navigate } from 'react-router-dom'
import Login from './Login'
import TwoFactor from './TwoFactor'

export default function App() {
    return (
        <Routes>
            {/* Корневой маршрут – страница логина */}
            <Route path="/" element={<Login />} />

            {/* Страница ввода 2FA */}
            <Route path="/2fa" element={<TwoFactor />} />

            {/* Все прочие – редирект на логин */}
            <Route path="*" element={<Navigate to="/" replace />} />
        </Routes>
    )
}

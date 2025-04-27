// src/main.jsx
import React from 'react';
import ReactDOM from 'react-dom/client';
import { BrowserRouter, Routes, Route } from 'react-router-dom';
import App from './App';
import TwoFactor from './TwoFactor';
import './App.css';

ReactDOM.createRoot(document.getElementById('root')).render(
    <BrowserRouter>
        <Routes>
            <Route path="/" element={<App />} />
            <Route path="/2fa" element={<TwoFactor />} />
            <Route path="/dashboard" element={<div>Защищённая страница</div>} />
        </Routes>
    </BrowserRouter>
);

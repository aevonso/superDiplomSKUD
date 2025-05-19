import React, { useState, useEffect, useRef } from 'react';
import { useParams, Link, useNavigate } from 'react-router-dom';
import QRCode from 'qrcode.react';
import html2canvas from 'html2canvas';
import jsPDF from 'jspdf';
import apiClient from './apiClient';
import logo from './assets/natk-logo.png';
import './RoomCard.css';

export default function RoomCard() {
    const { id } = useParams();
    const navigate = useNavigate();
    const qrRef = useRef();

    const [room, setRoom] = useState(null);
    const [floors, setFloors] = useState([]);
    const [loadingQR, setLoadingQR] = useState(false);

    const [form, setForm] = useState({
        name: '',
        floorId: ''
    });

    useEffect(() => {
        apiClient.get(`/api/Room/${id}`).then(r => {
            setRoom(r.data);
            setForm({ name: r.data.name, floorId: r.data.floorId });
        });
        apiClient.get('/api/Floor').then(r => setFloors(r.data));
    }, [id]);

    const handleChange = (field, val) => {
        setForm(f => ({ ...f, [field]: val }));
    };

    const handleUpdate = async () => {
        await apiClient.put(`/api/Room/${id}`, form);
        alert('Обновлено');
    };

    const handleDelete = async () => {
        if (window.confirm('Удалить помещение?')) {
            await apiClient.delete(`/api/Room/${id}`);
            navigate('/rooms');
        }
    };

    const generateQR = () => {
        setLoadingQR(true);
        setTimeout(() => {
            setLoadingQR(false);
        }, 1500);
    };

    const downloadPDF = async () => {
        const canvas = await html2canvas(qrRef.current);
        const imgData = canvas.toDataURL('image/png');
        const pdf = new jsPDF();
        pdf.addImage(imgData, 'PNG', 10, 10, 100, 100);
        pdf.save(`room_${room.id}_qr.pdf`);
    };

    if (!room) return <div>Загрузка...</div>;

    return (
        <div className="RoomCardPage">
            <header className="Header">
                <Link to="/rooms" className="BackLink">←</Link>
                <img src={logo} alt="НАТК" className="Header-logo" />
            </header>

            <main className="RoomCardMain">
                <h1>Карточка помещения</h1>

                <div className="Form">
                    <div className="Field">
                        <label>Название:</label>
                        <input
                            value={form.name}
                            onChange={e => handleChange('name', e.target.value)}
                        />
                    </div>

                    <div className="Field">
                        <label>Этаж:</label>
                        <select
                            value={form.floorId}
                            onChange={e => handleChange('floorId', e.target.value)}
                        >
                            {floors.map(f => (
                                <option key={f.id} value={f.id}>{f.name}</option>
                            ))}
                        </select>
                    </div>

                    <div className="Buttons">
                        <button className="Btn red" onClick={handleDelete}>Удалить помещение</button>
                        <button className="Btn orange" onClick={handleUpdate}>Обновить помещение</button>
                        <button className="Btn green" onClick={generateQR}>Сгенерировать QR-code помещения</button>
                    </div>

                    <div className="QRBox">
                        {loadingQR ? (
                            <div className="Spinner" />
                        ) : (
                            <div ref={qrRef}>
                                <QRCode value={`room-${room.id}`} size={200} />
                            </div>
                        )}
                    </div>

                    {!loadingQR && (
                        <button className="Btn blue" onClick={downloadPDF}>
                            Сгенерировать PDF-файл QR-code
                        </button>
                    )}
                </div>
            </main>
        </div>
    );
}

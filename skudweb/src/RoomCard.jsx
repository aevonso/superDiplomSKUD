import React, { useState, useEffect, useRef } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import QRCode from 'react-qr-code';
import html2canvas from 'html2canvas';
import jsPDF from 'jspdf';
import Swal from 'sweetalert2';

import {
    fetchFloors,
    fetchRoomsAll,
    fetchRoomById,
    fetchPostsWithAccess,
    updateRoom,
    deleteRoom,
    checkQrAccess
} from './accessMatrixApi';
import logo from './assets/natk-logo.png';
import './RoomCard.css';

export default function RoomCard() {
    const navigate = useNavigate();
    const qrRef = useRef();

    const [floors, setFloors] = useState([]);
    const [rooms, setRooms] = useState([]);
    const [roomId, setRoomId] = useState(null);
    const [room, setRoom] = useState(null);

    const [form, setForm] = useState({ name: '', floorId: '' });
    const [posts, setPosts] = useState([]);
    const [postId, setPostId] = useState('');
    const [loadingQR, setLoadingQR] = useState(false);
    const [showQR, setShowQR] = useState(false); // ✅ Появление QR-кода

    useEffect(() => {
        fetchFloors()
            .then(setFloors)
            .catch(() => Swal.fire('Ошибка', 'Не удалось загрузить этажи', 'error'));
    }, []);

    useEffect(() => {
        if (form.floorId) {
            fetchRoomsAll()
                .then(all => {
                    const filtered = all.filter(r => r.floor?.id === Number(form.floorId));
                    setRooms(filtered);
                    if (filtered.length > 0) {
                        setRoomId(filtered[0].id);
                    }
                })
                .catch(() => Swal.fire('Ошибка', 'Не удалось загрузить помещения', 'error'));
        }
    }, [form.floorId]);

    useEffect(() => {
        if (roomId) {
            fetchRoomById(roomId)
                .then(data => {
                    setRoom(data);
                    setForm({ name: data.name, floorId: data.floorId });
                })
                .catch(() => Swal.fire('Ошибка', 'Не удалось загрузить помещение', 'error'));

            fetchPostsWithAccess(roomId)
                .then(setPosts)
                .catch(() => Swal.fire('Ошибка', 'Не удалось загрузить должности', 'error'));
        }
    }, [roomId]);

    const handleChange = (field, val) => {
        setForm(f => ({ ...f, [field]: val }));
    };

    const handleUpdate = async () => {
        try {
            await updateRoom(roomId, form);
            Swal.fire('Успешно', 'Помещение обновлено', 'success');
        } catch {
            Swal.fire('Ошибка', 'Не удалось обновить помещение', 'error');
        }
    };

    const handleDelete = async () => {
        const res = await Swal.fire({
            title: 'Удалить помещение?',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Да',
            cancelButtonText: 'Отмена'
        });

        if (res.isConfirmed) {
            try {
                await deleteRoom(roomId);
                Swal.fire('Удалено', 'Помещение удалено', 'success');
                navigate('/rooms');
            } catch {
                Swal.fire('Ошибка', 'Не удалось удалить помещение', 'error');
            }
        }
    };

    const generateQR = () => {
        if (!postId) {
            Swal.fire('Ошибка', 'Выберите должность!', 'warning');
            return;
        }
        setShowQR(false);
        setLoadingQR(true);
        setTimeout(() => {
            setLoadingQR(false);
            setShowQR(true);
        }, 1000);
    };

    const downloadPDF = async () => {
        try {
            const canvas = await html2canvas(qrRef.current);
            const imgData = canvas.toDataURL('image/png');
            const pdf = new jsPDF();
            pdf.addImage(imgData, 'PNG', 10, 10, 100, 100);
            pdf.save(`room_${roomId}_qr.pdf`);
        } catch {
            Swal.fire('Ошибка', 'Не удалось сохранить PDF', 'error');
        }
    };

    const handleCheckAccess = async () => {
        if (!postId) {
            Swal.fire('Ошибка', 'Выберите должность!', 'warning');
            return;
        }

        try {
            const result = await checkQrAccess({ postId: Number(postId), roomId });
            Swal.fire(result.hasAccess ? '✅ ДОСТУП РАЗРЕШЁН' : '❌ ДОСТУП ЗАПРЕЩЁН', '', result.hasAccess ? 'success' : 'error');
        } catch {
            Swal.fire('Ошибка', 'Ошибка при проверке доступа', 'error');
        }
    };

    const qrValue = JSON.stringify({ postId: Number(postId), roomId });

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
                        <label>Этаж:</label>
                        <select value={form.floorId} onChange={e => handleChange('floorId', e.target.value)}>
                            <option value="">— выберите этаж —</option>
                            {floors.map(f => (
                                <option key={f.id} value={f.id}>{f.name}</option>
                            ))}
                        </select>
                    </div>

                    <div className="Field">
                        <label>Помещение:</label>
                        <select value={roomId ?? ''} onChange={e => setRoomId(Number(e.target.value))}>
                            <option value="">— выберите помещение —</option>
                            {rooms.map(r => (
                                <option key={r.id} value={r.id}>{r.name}</option>
                            ))}
                        </select>
                    </div>

                    <div className="Field">
                        <label>Название:</label>
                        <input value={form.name} onChange={e => handleChange('name', e.target.value)} />
                    </div>

                    <div className="Field">
                        <label>Должность:</label>
                        <select value={postId} onChange={e => setPostId(e.target.value)}>
                            <option value="">— выберите —</option>
                            {posts.map(p => (
                                <option key={p.id} value={p.id}>
                                    {p.name} ({p.division}) {p.hasAccess ? '✅' : '❌'}
                                </option>
                            ))}
                        </select>
                    </div>

                    <div className="Buttons">
                        <button className="Btn red" onClick={handleDelete}>Удалить помещение</button>
                        <button className="Btn orange" onClick={handleUpdate}>Обновить помещение</button>
                        <button className="Btn green" onClick={generateQR}>Сгенерировать QR-code</button>
                        <button className="Btn blue" onClick={handleCheckAccess}>Проверить доступ</button>
                    </div>

                    <div className="QRBox">
                        {loadingQR ? (
                            <div className="Spinner" />
                        ) : (
                            showQR && (
                                <div ref={qrRef}>
                                    <QRCode value={qrValue} size={200} />
                                </div>
                            )
                        )}
                    </div>

                    {showQR && (
                        <button className="Btn blue" onClick={downloadPDF}>
                            Сгенерировать PDF QR-code
                        </button>
                    )}
                </div>
            </main>
        </div>
    );
}

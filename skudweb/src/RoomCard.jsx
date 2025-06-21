import React, { useEffect, useState, useRef } from 'react';
import { Link, useNavigate, useParams } from 'react-router-dom';
import Swal from 'sweetalert2';
import QRCode from 'react-qr-code';
import html2canvas from 'html2canvas';
import jsPDF from 'jspdf';

import {
    fetchFloors,
    fetchRoomsAll,
    fetchRoomById,
    fetchPostsWithAccess,
    updateRoom,
    deleteRoom
} from './accessMatrixApi';
import logo from './assets/natk-logo.png';
import './RoomCard.css';

export default function RoomCard() {
    const { id } = useParams();
    const navigate = useNavigate();
    const qrRef = useRef();

    const [floors, setFloors] = useState([]);
    const [rooms, setRooms] = useState([]);
    const [roomId, setRoomId] = useState(Number(id) || null);
    const [form, setForm] = useState({ name: '', floorId: '' });
    const [posts, setPosts] = useState([]);    // сюда попадут только доступные

    const [loadingQR, setLoadingQR] = useState(false);
    const [showQR, setShowQR] = useState(false);

    // 1) Загрузка этажей
    useEffect(() => {
        fetchFloors().then(setFloors)
            .catch(() => Swal.fire('Ошибка', 'Не удалось загрузить этажи', 'error'));
    }, []);

    // 2) Когда выбирают этаж — подтягиваем комнаты
    useEffect(() => {
        if (!form.floorId) return;
        fetchRoomsAll()
            .then(all => {
                const fl = all.filter(r => r.floor?.id === Number(form.floorId));
                setRooms(fl);
                if (!roomId && fl.length) setRoomId(fl[0].id);
            })
            .catch(() => Swal.fire('Ошибка', 'Не удалось загрузить помещения', 'error'));
    }, [form.floorId]);

    // 3) Когда меняется комната — грузим данные и матрицу доступа
    useEffect(() => {
        if (!roomId) return;

        fetchRoomById(roomId)
            .then(data => setForm({ name: data.name, floorId: data.floorId }))
            .catch(() => Swal.fire('Ошибка', 'Не удалось загрузить данные помещения', 'error'));

        // здесь фильтруем только те должности, у которых hasAccess = true
        fetchPostsWithAccess(roomId)
            .then(arr => setPosts(arr.filter(p => p.hasAccess)))
            .catch(() => Swal.fire('Ошибка', 'Не удалось загрузить матрицу доступа', 'error'));
    }, [roomId]);

    const handleChange = (field, v) => {
        setForm(f => ({ ...f, [field]: v }));
    };

    const handleUpdate = async () => {
        try {
            await updateRoom(roomId, form);
            Swal.fire('Успешно', 'Комната сохранена', 'success');
        } catch {
            Swal.fire('Ошибка', 'Не удалось сохранить', 'error');
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
        if (!res.isConfirmed) return;
        try {
            await deleteRoom(roomId);
            Swal.fire('Удалено', 'Помещение удалено', 'success');
            navigate('/rooms');
        } catch {
            Swal.fire('Ошибка', 'Не удалось удалить', 'error');
        }
    };

    const generateQR = () => {
        setShowQR(false);
        setLoadingQR(true);
        setTimeout(() => {
            setLoadingQR(false);
            setShowQR(true);
        }, 300);
    };

    const downloadPDF = async () => {
        try {
            const canvas = await html2canvas(qrRef.current, { scale: 2 });
            const img = canvas.toDataURL('image/png');
            const pdf = new jsPDF();
            const w = pdf.internal.pageSize.getWidth();
            const h = (canvas.height * w) / canvas.width;
            pdf.addImage(img, 'PNG', 0, 0, w, h);
            pdf.save(`room_${roomId}_qr.pdf`);
        } catch {
            Swal.fire('Ошибка', 'Не удалось сохранить PDF', 'error');
        }
    };

    const qrValue = JSON.stringify({ roomId });

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
                            {floors.map(f => <option key={f.id} value={f.id}>{f.name}</option>)}
                        </select>
                    </div>

                    <div className="Field">
                        <label>Помещение:</label>
                        <select value={roomId || ''} onChange={e => setRoomId(Number(e.target.value))}>
                            <option value="">— выберите помещение —</option>
                            {rooms.map(r => <option key={r.id} value={r.id}>{r.name}</option>)}
                        </select>
                    </div>

                    <div className="Field">
                        <label>Название:</label>
                        <input value={form.name} onChange={e => handleChange('name', e.target.value)} />
                    </div>

                    <div className="Buttons">
                        <button className="Btn red" onClick={handleDelete}>Удалить</button>
                        <button className="Btn orange" onClick={handleUpdate}>Обновить</button>
                        <button className="Btn green" onClick={generateQR}>Показать QR</button>
                    </div>

                    <div className="QRBox">
                        {loadingQR
                            ? <div className="Spinner" />
                            : showQR && (
                                <div ref={qrRef} className="QRWrapper">
                                    <div className="QRHeader">QR-код помещения №{form.name}</div>
                                    <QRCode value={qrValue} size={120} />
                                </div>
                            )
                        }
                    </div>

                    {showQR && (
                        <button className="Btn blue" onClick={downloadPDF}>
                            Сохранить PDF
                        </button>
                    )}
                </div>
            </main>
        </div>
    );
}
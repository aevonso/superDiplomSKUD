import apiClient from './apiClient';

/**
 * Возвращает все этажи: [{ id, name }, …]
 */
export async function fetchFloors() {
    const { data } = await apiClient.get('/api/Floor');
    return data;
}

/**
 * Возвращает все подразделения: [{ id, name }, …]
 */
export async function fetchDivisions() {
    const { data } = await apiClient.get('/api/Division');
    return data;
}

/**
 * Возвращает все комнаты (без фильтра)
 */
export async function fetchRoomsAll() {
    const { data } = await apiClient.get('/api/Room');
    return data;
}

/**
 * Возвращает комнаты заданного этажа
 */
export async function fetchRoomsByFloor(floorId) {
    const { data } = await apiClient.get(`/api/Room?floorId=${floorId}`);
    return data;
}

/**
 * Возвращает записи матрицы доступа, с возможной фильтрацией
 * по этажу и подразделению:
 *   [{ post: { id, name, division: { … } }, room: { id, name }, isAccess }, …]
 */
export async function fetchAccessMatrix({ floorId, divisionId }) {
    const q = new URLSearchParams();
    if (floorId != null) q.append('floorId', floorId);
    if (divisionId != null) q.append('divisionId', divisionId);
    const { data } = await apiClient.get(`/api/AccessMatrix?${q}`);
    return data;
}

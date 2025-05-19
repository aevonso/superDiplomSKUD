import apiClient from './apiClient';

/**
 * Возвращает все этажи
 */
export async function fetchFloors() {
    const { data } = await apiClient.get('/api/Floor');
    return data;
}

/**
 * Возвращает все подразделения
 */
export async function fetchDivisions() {
    const { data } = await apiClient.get('/api/Division');
    return data;
}

/**
 * Возвращает все должности с подразделениями
 */
export async function fetchPostsAll() {
    const { data } = await apiClient.get('/api/Post');
    return data;
}

/**
 * Возвращает все комнаты с этажами
 */
export async function fetchRoomsAll() {
    const { data } = await apiClient.get('/api/Room');
    return data;
}

/**
 * Возвращает матрицу доступа с фильтрацией
 */
export async function fetchAccessMatrix({ floorId, divisionId }) {
    const q = new URLSearchParams();
    if (floorId != null) q.append('floorId', floorId);
    if (divisionId != null) q.append('divisionId', divisionId);
    const { data } = await apiClient.get(`/api/AccessMatrix?${q}`);
    return data;
}

/**
 * Создать новую запись доступа
 */
export async function createAccessMatrixEntry(entry) {
    const { data } = await apiClient.post('/api/AccessMatrix', entry);
    return data;
}

/**
 * Обновить запись доступа по ID
 */
export async function updateAccessMatrixEntry(id, entry) {
    const { data } = await apiClient.put(`/api/AccessMatrix/${id}`, {
        id: id,
        postId: entry.postId,
        roomId: entry.roomId,
        isAccess: entry.isAccess
    });
    return data;
}

/**
 * Переключить доступ (toggle) по ID
 */
export async function toggleAccessMatrixEntry(id) {
    await apiClient.put(`/api/AccessMatrix/${id}/toggle`);
}

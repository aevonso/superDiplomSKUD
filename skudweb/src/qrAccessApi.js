import apiClient from './apiClient';

/**
 * Получить помещение по ID
 */
export async function fetchRoomById(id) {
    const { data } = await apiClient.get(`/api/Room/${id}`);
    return data;
}

/**
 * Обновить помещение
 */
export async function updateRoom(id, form) {
    return apiClient.put(`/api/Room/${id}`, form);
}

/**
 * Удалить помещение
 */
export async function deleteRoom(id) {
    return apiClient.delete(`/api/Room/${id}`);
}

/**
 * Получить список этажей
 */
export async function fetchFloors() {
    const { data } = await apiClient.get('/api/Floor');
    return data;
}

/**
 * Проверка доступа сотрудника к помещению через QR
 * @param {{postId: number, roomId: number}} payload
 * @returns {Promise<{hasAccess: boolean}>}
 */
export async function checkQrAccess(payload) {
    const { data } = await apiClient.post('/api/QrAccess/check', payload);
    return data;
}

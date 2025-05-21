import apiClient from './apiClient';

/**
 * �������� ��������� �� ID
 */
export async function fetchRoomById(id) {
    const { data } = await apiClient.get(`/api/Room/${id}`);
    return data;
}

/**
 * �������� ���������
 */
export async function updateRoom(id, form) {
    return apiClient.put(`/api/Room/${id}`, form);
}

/**
 * ������� ���������
 */
export async function deleteRoom(id) {
    return apiClient.delete(`/api/Room/${id}`);
}

/**
 * �������� ������ ������
 */
export async function fetchFloors() {
    const { data } = await apiClient.get('/api/Floor');
    return data;
}

/**
 * �������� ������� ���������� � ��������� ����� QR
 * @param {{postId: number, roomId: number}} payload
 * @returns {Promise<{hasAccess: boolean}>}
 */
export async function checkQrAccess(payload) {
    const { data } = await apiClient.post('/api/QrAccess/check', payload);
    return data;
}

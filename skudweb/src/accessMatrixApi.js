import apiClient from './apiClient';

/**
 * ���������� ��� �����
 */
export async function fetchFloors() {
    const { data } = await apiClient.get('/api/Floor');
    return data;
}

/**
 * ���������� ��� �������������
 */
export async function fetchDivisions() {
    const { data } = await apiClient.get('/api/Division');
    return data;
}

/**
 * ���������� ��� ��������� � ���������������
 */
export async function fetchPostsAll() {
    const { data } = await apiClient.get('/api/Post');
    return data;
}

/**
 * ���������� ��� ������� � �������
 */
export async function fetchRoomsAll() {
    const { data } = await apiClient.get('/api/Room');
    return data;
}

/**
 * ���������� ������� ������� � �����������
 */
export async function fetchAccessMatrix({ floorId, divisionId }) {
    const q = new URLSearchParams();
    if (floorId != null) q.append('floorId', floorId);
    if (divisionId != null) q.append('divisionId', divisionId);
    const { data } = await apiClient.get(`/api/AccessMatrix?${q}`);
    return data;
}

/**
 * ������� ����� ������ �������
 */
export async function createAccessMatrixEntry(entry) {
    const { data } = await apiClient.post('/api/AccessMatrix', entry);
    return data;
}

/**
 * �������� ������ ������� �� ID
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
 * ����������� ������ (toggle) �� ID
 */
export async function toggleAccessMatrixEntry(id) {
    await apiClient.put(`/api/AccessMatrix/${id}/toggle`);
}

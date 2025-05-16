import apiClient from './apiClient';

/**
 * ���������� ��� �����: [{ id, name }, �]
 */
export async function fetchFloors() {
    const { data } = await apiClient.get('/api/Floor');
    return data;
}

/**
 * ���������� ��� �������������: [{ id, name }, �]
 */
export async function fetchDivisions() {
    const { data } = await apiClient.get('/api/Division');
    return data;
}

/**
 * ���������� ��� ������� (��� �������)
 */
export async function fetchRoomsAll() {
    const { data } = await apiClient.get('/api/Room');
    return data;
}

/**
 * ���������� ������� ��������� �����
 */
export async function fetchRoomsByFloor(floorId) {
    const { data } = await apiClient.get(`/api/Room?floorId=${floorId}`);
    return data;
}

/**
 * ���������� ������ ������� �������, � ��������� �����������
 * �� ����� � �������������:
 *   [{ post: { id, name, division: { � } }, room: { id, name }, isAccess }, �]
 */
export async function fetchAccessMatrix({ floorId, divisionId }) {
    const q = new URLSearchParams();
    if (floorId != null) q.append('floorId', floorId);
    if (divisionId != null) q.append('divisionId', divisionId);
    const { data } = await apiClient.get(`/api/AccessMatrix?${q}`);
    return data;
}

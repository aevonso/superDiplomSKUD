import apiClient from './apiClient';

/**
 * �������� ���������� � �����������
 * @param {{
 *   date?: string,
 *   employeeName?: string,
 *   deviceName?: string
 * }} filters
 * @returns {Promise<Array<{
 *   id: number,
 *   deviceName: string,
 *   createdAt: string,
 *   isActive: boolean,
 *   employeeName: string
 * }>>}
 */
export async function fetchMobileDevices(filters = {}) {
    const params = new URLSearchParams();
    if (filters.date) params.append('date', filters.date);
    if (filters.employeeName) params.append('employeeName', filters.employeeName);
    if (filters.deviceName) params.append('deviceName', filters.deviceName);

    const { data } = await apiClient.get(`/api/MobileDevices?${params}`);
    return data;
}

/**
 * �������� ����� ����������
 * @param {{ employerId: number, deviceName: string }} payload
 * @returns {Promise<number>} � ID ������ ����������
 */
export async function addMobileDevice(payload) {
    const { data } = await apiClient.post('/api/MobileDevices', payload);
    return data;
}

/**
 * ������� ���������� �� ID
 * @param {number} id
 * @returns {Promise<void>}
 */
export async function deleteMobileDevice(id) {
    await apiClient.delete(`/api/MobileDevices/${id}`);
}

/**
 * �������� ����������
 * @param {number} id
 * @param {{ employerId: number, deviceName: string, isActive: boolean }} payload
 * @returns {Promise<void>}
 */
export async function updateMobileDevice(id, payload) {
    await apiClient.put(`/api/MobileDevices/${id}`, payload);
}

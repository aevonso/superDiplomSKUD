import apiClient from './apiClient';

/**
 * �������� ������ ��������� � �����������
 * @param {Object} filters
 * @param {string} [filters.date]
 * @param {string} [filters.employeeName]
 * @param {string} [filters.deviceName]
 * @returns {Promise<Array<{
 *   id: number,
 *   deviceName: string,
 *   deviceCode: string,
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
 * �������� ���������� �� ID
 * @param {number|string} id
 * @returns {Promise<{
 *   id: number,
 *   deviceName: string,
 *   deviceCode: string,
 *   createdAt: string,
 *   isActive: boolean,
 *   employeeName: string
 * }>}
 */
export async function fetchMobileDeviceById(id) {
    const { data } = await apiClient.get(`/api/MobileDevices/${id}`);
    return data; // data.deviceCode ������ ��������� �� API
}

/**
 * �������� ����� ����������
 * @param {{ employerId: number, deviceName: string, deviceCode: string }} payload
 * @returns {Promise<number>} � ID ������ ����������
 */
export async function addMobileDevice(payload) {
    const { data } = await apiClient.post('/api/MobileDevices', payload);
    return data;
}

/**
 * �������� ���������� �� ID
 * @param {number|string} id
 * @param {{ deviceName: string, deviceCode: string }} payload
 * @returns {Promise<void>}
 */
export async function updateMobileDevice(id, payload) {
    const { data } = await apiClient.put(`/api/MobileDevices/${id}`, payload);
    return data; // ������ � ����� deviceCode
}

/**
 * ������� ���������� �� ID
 * @param {number|string} id
 * @returns {Promise<void>}
 */
export async function deleteMobileDevice(id) {
    await apiClient.delete(`/api/MobileDevices/${id}`);
}

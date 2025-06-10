import apiClient from './apiClient';

/**
 * Получить список устройств с фильтрацией
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
 * Получить устройство по ID
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
    return data; // data.deviceCode должен приходить из API
}

/**
 * Добавить новое устройство
 * @param {{ employerId: number, deviceName: string, deviceCode: string }} payload
 * @returns {Promise<number>} — ID нового устройства
 */
export async function addMobileDevice(payload) {
    const { data } = await apiClient.post('/api/MobileDevices', payload);
    return data;
}

/**
 * Обновить устройство по ID
 * @param {number|string} id
 * @param {{ deviceName: string, deviceCode: string }} payload
 * @returns {Promise<void>}
 */
export async function updateMobileDevice(id, payload) {
    const { data } = await apiClient.put(`/api/MobileDevices/${id}`, payload);
    return data; // строка с новым deviceCode
}

/**
 * Удалить устройство по ID
 * @param {number|string} id
 * @returns {Promise<void>}
 */
export async function deleteMobileDevice(id) {
    await apiClient.delete(`/api/MobileDevices/${id}`);
}

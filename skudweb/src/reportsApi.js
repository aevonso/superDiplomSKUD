import apiClient from './apiClient';

/**
 * ��������� ������ � �������� ������� (PDF ��� DOCX).
 * @param {'pdf'|'docx'} format
 * @param {Object} options - ����� ��� ������������ ������
 * @returns {Blob} - Blob � �������
 */
export async function generateReport(format, options) {
    // ����������� ������ ��� � ������� Date ����� ���������
    const fromDate = options.fromDate ? new Date(options.fromDate) : null;
    const toDate = options.toDate ? new Date(options.toDate) : null;

    const resp = await apiClient.post(
        '/api/reports',
        {
            includeAccessAttempts: options.includeAccessAttempts,
            successOnly: options.successOnly,
            failedOnly: options.failedOnly,
            fromDate: fromDate ? fromDate.toISOString() : null, // ����������� � ISO ������
            toDate: toDate ? toDate.toISOString() : null, // ����������� � ISO ������
            format: format
        },
        { responseType: 'blob' }
    );
    return resp.data;
}

import axiosClient from './axiosClient'

export async function getCurrentShift() {
  const response = await axiosClient.get('/shifts/current')
  return response.data
}

export async function clockIn() {
  const response = await axiosClient.post('/shifts/clock-in')
  return response.data
}

export async function clockOut() {
  const response = await axiosClient.post('/shifts/clock-out')
  return response.data
}

export async function getShiftHistory() {
  const response = await axiosClient.get('/shifts/history')
  return response.data
}

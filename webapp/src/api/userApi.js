import axios from 'axios';

const API_BASE_URL = 'http://localhost:5147';  

export const getAdminProfiles = (token) => {
  return axios.get(`${API_BASE_URL}/api/user/all/admin`, {
    headers: {
      Authorization: `Bearer ${token}`,
    },
  });
};

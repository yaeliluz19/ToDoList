import axios from 'axios';
axios.defaults.baseURL = 'http://localhost:5219/';
axios.defaults.headers['Content-Type'] = 'application/json';

axios.interceptors.request.use((config) => {

  const token = sessionStorage.getItem('token');
  if (token) {
    config.headers['Authorization'] = `Bearer ${token}`;
  }
  return config;
}, (error) => {
  return Promise.reject(error);
});
axios.interceptors.response.use(
  (response) => response,
  (error) => {
    console.error('API Error:', error.response || error.message);
    if (error.response && error.response.status === 401) {
      sessionStorage.removeItem('token');
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

const apiUrl = "http://localhost:5219/";

export default {
  getTasks: async () => {
    try {
      const result = await axios.get(`${apiUrl}`);
      return result.data;
    } catch (error) {
      console.error("Error fetching tasks:", error);
      throw error;
    }
  },

  addTask: async (name) => {
    const newTask = {
      name: name,
      isComplete: false
    };
    try {
      const token = sessionStorage.getItem('token');
      if (!token) {
        alert(new Error('Token is missing. Please log in again.'));
        window.location.href = '/login';      }
      await axios.post(`${apiUrl}`, newTask);
    } catch (error) {
      console.error("Error adding task:", error);
      throw error;
    }
  },

  setCompleted: async (id, isComplete) => {
    try {
      const token = sessionStorage.getItem('token');
      if (!token) {
        alert(new Error('Token is missing. Please log in again.'));
        window.location.href = '/login';
      }
      await axios.put(`${apiUrl}${id}`, { isComplete: isComplete });
    } catch (error) {
      console.error("Error updating task:", error);
      throw error;
    }
  },

  deleteTask: async (id) => {
    try {
      const token = sessionStorage.getItem('token');
      if (!token) {
        alert(new Error('Token is missing. Please log in again.'));
        window.location.href = '/login';
      }
      await axios.delete(`${apiUrl}${id}`);
    }
    catch (error) {
      console.error("Error deleting task:", error);
      throw error;
    }
  }
};

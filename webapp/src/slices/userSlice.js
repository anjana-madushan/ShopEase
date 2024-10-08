import { createSlice } from '@reduxjs/toolkit';
import axios from 'axios';

// Initialize state from localStorage if available
const tokenFromStorage = localStorage.getItem('token');
const userFromStorage = localStorage.getItem('user');
const roleFromStorage = localStorage.getItem('role');

const initialState = {
  isLoggedIn: !!tokenFromStorage, // true if token exists in localStorage
  user: userFromStorage ? JSON.parse(userFromStorage) : null,
  token: tokenFromStorage || null,
  role: roleFromStorage || null,
  loading: false,
  error: null,
};

const userSlice = createSlice({
  name: 'user',
  initialState,
  reducers: {
    loginStart: (state) => {
      state.loading = true;
      state.error = null;
    },
    loginSuccess: (state, action) => {
      state.isLoggedIn = true;
      state.user = action.payload.user;
      state.token = action.payload.token;
      state.role = action.payload.role;
      state.loading = false;
      state.error = null;
    },
    loginFailure: (state, action) => {
      state.loading = false;
      state.error = action.payload;
    },
    logout: (state) => {
      state.isLoggedIn = false;
      state.user = null;
      state.token = null;
      state.role = null; // Clear role from Redux state
      localStorage.removeItem('token');
      localStorage.removeItem('user');
      localStorage.removeItem('role'); // Remove role from localStorage as well
    },
  },
});

export const { loginStart, loginSuccess, loginFailure, logout } = userSlice.actions;

// Thunk for logging in the user
export const loginUser = (email, password, role) => async (dispatch) => {
  dispatch(loginStart());
  try {
    const response = await axios.post('http://localhost:5147/api/user/login', {
      email,
      password,
      role,
    });

    let user;
    // Handle different roles based on the response
    if (role === 'admin') {
      user = response.data.admin;
    } else if (role === 'vendor') {
      user = response.data.vendor;
    } else if (role === 'csr') {
      user = response.data.csr;
    }

    const { token } = response.data;

    // Ensure that user data and token exist
    if (user && token) {
      dispatch(loginSuccess({ user, token, role }));

      // Store token, user, and role in localStorage for persistence
      localStorage.setItem('token', token);
      localStorage.setItem('user', JSON.stringify(user));
      localStorage.setItem('role', role); // Store role in localStorage
    } else {
      throw new Error('Login failed: Invalid response from server');
    }

  } catch (error) {
    const errorMessage = error.response?.data?.message || 'Login failed';
    console.error("Login Error:", errorMessage);
    dispatch(loginFailure(errorMessage));
  }
};

// Thunk for logging out the user
export const logoutUser = () => (dispatch) => {
  dispatch(logout());
  // Additional clean-up actions (like redirecting to login page) can be added here if needed
};

export default userSlice.reducer;

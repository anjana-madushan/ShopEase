import { createSlice } from "@reduxjs/toolkit";

// Define a function to fetch the initial state
const getInitialAuthState = () => {
  const persistedState = localStorage.getItem("persist:root");
  if (persistedState) {
    const parsedState = JSON.parse(persistedState);
    return parsedState.auth || {}; // Assuming your persisted state has an 'auth' property
  }

  return {
    message: "",
    loggedUser: {
      id: "",
      username: "",
      email: "",
      role: "",
      token: "",
    },
  };
};

export const authSlice = createSlice({
  name: "auth",
  initialState: getInitialAuthState(),
  reducers: {
    setLoginResponse: (state, action) => {
      // Map the server response to the loggedUser state
      const { user, token } = action.payload;
      state.loggedUser = {
        id: user.id,
        username: user.username,
        email: user.email,
        role: user.role,
        token: token,
      };
      state.message = "Login successful";
    },
    setMessage: (state, action) => {
      state.message = action.payload;
    },
    clearMessage: (state) => {
      state.message = "";
    },
    logOut: (state) => {
      // Reset auth state on logout
      state.loggedUser = {
        id: "",
        username: "",
        email: "",
        role: "",
        token: "",
      };
      state.message = "";
    },
    resetState: () => {
      // Clear entire Redux state on reset
      localStorage.removeItem("persist:root");
      return {
        message: "",
        loggedUser: {
          id: "",
          username: "",
          email: "",
          role: "",
          token: "",
        },
      };
    },
  },
});

export const {
  setLoginResponse,
  logOut,
  setMessage,
  clearMessage,
  resetState,
} = authSlice.actions;

export default authSlice.reducer;

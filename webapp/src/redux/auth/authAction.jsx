/* eslint-disable no-unused-vars */
import {
  authSlice,
  resetState,
  setLoginResponse,
  setMessage,
} from "./authSlice";
import {signIn } from "../../api/services/authService";
import { useNavigate } from "react-router-dom";

const authActions = authSlice.actions;

//Login
export const loginAction = (email, password, role, navigate) => {
    return async (dispatch) => {
      try {
        const response = await signIn(email, password, role);
        if (response && response.user && response.token) {
          dispatch(setLoginResponse({
            user: response.user,
            token: response.token
          }));
          alert("Login successful");
          navigate("/home");
        } else {
          throw new Error("Invalid response from server");
        }
      } catch (error) {
        alert("Error logging in");
        console.error("Login error:", error);
        dispatch(setMessage(error.message || "An error occurred while logging in."));
      }
    };
  };
  
  
//Sign Out
export const signOutAction = () => {
  return async (dispatch) => {
    try {
      dispatch(resetState());
      const navigate = useNavigate();
      navigate("/");
    } catch (error) {
      console.error("Sign out error:", error);
      if (
        error.response &&
        error.response.data &&
        error.response.data.message
      ) {
        dispatch(setMessage(error.response.data.message));
      } else {
        console.error("Sign out error:", error);
        dispatch(setMessage("An error occurred while signing out."));
      }
    }
  };
};

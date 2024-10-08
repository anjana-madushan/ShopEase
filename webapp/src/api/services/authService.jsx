import { apiClient } from "../axios/api";

//Login
export const signIn = async (email, password, role) => {
  try {
    const response = await apiClient.post("/api/user/login", {
      email,
      password,
      role,
    });
    return response.data;
  } catch (error) {
    console.log(error);
    return error.response.data;
  }
};

//Get all users based on role
export const getAllUsers = async (token, role) => {
  try {
    console.log("Tokenapi:", token);
    const response = await apiClient.get(`/api/user/all/${role}`, {
      headers: {
        Authorization: `${token}`,
      },
    });
    return response.data;
  } catch (error) {
    console.log(error);
    return error.response.data;
  }
};

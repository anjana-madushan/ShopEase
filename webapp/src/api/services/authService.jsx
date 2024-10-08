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
    //console.log("Tokenapi:", token);
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

export const getAllAdmins = async (token) => {
  try {
    //console.log("Tokenapi:", token);
    const response = await apiClient.get(`/api/user/all/admin`, {
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
export const getAllCsrs = async (token) => {
  try {
    //console.log("Tokenapi:", token);
    const response = await apiClient.get(`/api/user/all/csr`, {
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
export const getAllVendors = async (token) => {
  try {
    //console.log("Tokenapi:", token);
    const response = await apiClient.get(`/api/user/all/vendor`, {
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

export const createAdmin = async (username, password, email, token) => {
  try {
    const response = await apiClient.post(`/api/admin/create/admin`, 
      {
        username,
        password,
        email,
      }, 
      {
        headers: {
          Authorization: `${token}`,
        },
      }
    );
    return response.data;
  } catch (error) {
    console.log(error);
    return error.response.data;
  }
}
export const createCsr = async (username, password, email, token) => {
  try {
    const response = await apiClient.post(`/api/admin/create/csr`, 
      {
        username,
        password,
        email,
      }, 
      {
        headers: {
          Authorization: `${token}`,
        },
      }
    );
    return response.data;
  } catch (error) {
    console.log(error);
    return error.response.data;
  }
}
export const createVender = async (username, password, email, token) => {
  try {
    const response = await apiClient.post(`/api/admin/create/vendor`, 
      {
        username,
        password,
        email,
      }, 
      {
        headers: {
          Authorization: `${token}`,
        },
      }
    );
    return response.data;
  } catch (error) {
    console.log(error);
    return error.response.data;
  }
}
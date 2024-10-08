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

export const createUser = async (username, password, email, token , role) => {
  try {
    const response = await apiClient.post(`/api/admin/create/${role}`, 
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

export const getApprovedCus = async (token) => {
  try {
    //console.log("Tokenapi:", token);
    const response = await apiClient.get(`/api/user/approved/customers`, {
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

export const getUnapprovedCus = async (token) => {
  try {
    //console.log("Tokenapi:", token);
    const response = await apiClient.get(`/api/user/unapproved/customers`, {
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

export const getUserById = async (id, token) => {
  try {
    const response = await apiClient.get(`api/user/admin/${id}`, {
      headers: {
        Authorization: `${token}`,
      },
    });
    return response.data;
  } catch (error) {
    console.log(error);
    return error.response.data;
  }
}

export const getUserByEmail = async (email, token) => {
  try {
    const response = await apiClient.get(`/api/user/admin/email/${email}`, {
      headers: {
        Authorization: `${token}`,
      },
    });
    return response.data;
  } catch (error) {
    console.log(error);
    return error.response.data;
  }
}
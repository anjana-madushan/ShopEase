import { apiClient } from "../axios/api";

export const getAllOrders = async (token) => {
  try {
    const response = await apiClient.get(`api/orders/all-orders`, {
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

export const approveToCancelOrder = async (role, userId, token, orderId) => {
  try {
    const response = await apiClient.post(
      `api/orders/approve-cancel-order/${orderId}`,
      {
        role,
        userId,
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
};

export const rejectToCancelOrder = async (role, userId, token, orderId) => {
  try {
    const response = await apiClient.post(
      `/api/orders/reject-request-to-cancel-order/${orderId}`,
      {
        role,
        userId,
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
};

export const updateStatusDelivery = async (role, userId, token, orderId) => {
  try {
    const response = await apiClient.post(
      `/api/orders/order-status-delivered/${orderId}`,
      {
        role,
        userId,
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
};

export const updateStatusReady = async (role, userId, token, orderId) => {
  try {
    const response = await apiClient.post(
      `/api/orders/order-status-ready/${orderId}`,
      {
        role,
        userId,
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
};

export const updateStatusDispatch = async (role, userId, token, orderId) => {
  try {
    const response = await apiClient.post(
      `/api/orders/order-status-dispatched/${orderId}`,
      {
        role,
        userId,
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
};

export const getOrderByCustomerID = async (token, cusId) => {
  try {
    const response = await apiClient.post(
      `/api/orders/orders-by-user/${cusId}`,
      {
        role,
        userId,
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
};

export const getAllCanceledOrders = async (token) => {
  try {
    const response = await apiClient.get(`/api/orders/cancelled-orders`, {
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

export const getcanceledOrderByScr = async (token, scrId) => {
  try {
    const response = await apiClient.get(
      `/api/orders/orders-cancelled-by-csr/${scrId}`,
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
};

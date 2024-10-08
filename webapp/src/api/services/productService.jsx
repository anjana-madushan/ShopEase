import { apiClient } from "../axios/api";

//Get all Products based on vendor
export const getAllProducts = async (token) => {
  try {
    const response = await apiClient.get(`/api/Product/vender`, {
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

export const getAllProductsAdmin = async (token) => {
  try {
    const response = await apiClient.get(`/api/Product`, {
      headers: {
        Authorization: `${token}`,
      },
    });
    console.log(response.data);
    return response.data;
  } catch (error) {
    console.log(error);
    return error.response.data;
  }
};

export const getProductByID = async (productId, token) => {
  try {
    const response = await apiClient.get(`/api/Product/${productId}`, {
      headers: {
        Authorization: `${token}`,
      },
    });
    console.log(response)
    return response.data;
  } catch (error) {
    console.log(error);
    return error.response.data;
  }
};


export const updateProductByID = async (productId, product, token) => {
  try {
    console.log(product);
    const response = await apiClient.put(`/api/Product/${productId}`,
      product,
      {
        headers: {
          Authorization: `${token}`,
        },
      }
    );
    return response.data;
  } catch (error) {
    console.log(error);
    return error.response ? error.response.data : error.message;
  }
};

export const deleteProductByID = async (productId, token) => {
  try {
    const response = await apiClient.delete(`/api/Product/${productId}`,
      {
        headers: {
          Authorization: `${token}`,
        },
      }
    );
    return response;
  } catch (error) {
    console.log(error);
    return error.response ? error.response.data : error.message;
  }
};


export const createProduct = async (productName, price, category, description, isActive, stockLevel, minStockLevel, token) => {
  try {
    const response = await apiClient.post(`/api/Product`,
      {
        productName, price, category, description, isActive, stockLevel, minStockLevel
      },
      {
        headers: {
          Authorization: `${token}`,
        },
      }
    );
    console.log(response)
    return response.data;
  } catch (error) {
    console.log(error);
    return error.response.data;
  }
}
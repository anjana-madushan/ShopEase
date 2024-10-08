import { apiClient } from "../axios/api";

//Get all Comments based on vendor
export const getCommentsBasedOnVendor = async (token) => {
  try {
    const response = await apiClient.get(`/api/Comment/vender`, {
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
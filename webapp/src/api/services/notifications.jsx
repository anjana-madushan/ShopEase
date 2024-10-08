import { apiClient } from "../axios/api";

//Get all Products based on vendor
export const getNotificationsBasedOnVendor = async (token) => {
  try {
    const response = await apiClient.get(`/api/notification`, {
      headers: {
        Authorization: `${token}`,
      },
    });
    console.log(response)
    return response;
  } catch (error) {
    console.log(error);
    return error.response.data;
  }
};
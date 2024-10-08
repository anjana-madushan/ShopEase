import { createSlice } from "@reduxjs/toolkit";

// Define a function to fetch the initial state
const getInitialOrderState = () => {
  const persistedState = localStorage.getItem("persist:root");
  if (persistedState) {
    const parsedState = JSON.parse(persistedState);
    return parsedState.order || {}; // Assuming your persisted state has an 'order' property
  }

  return {
    id: "",
    orderId: "",
    orderDate: "",
    status: 0,
    statusUpdatedOn: "",
    shippingAddress: "",
    billingAddress: "",
    email: "",
    products: {},
    totalPrice: 0,
    totalQty: 0,
    requestToCancel: false,
    cancelled: false,
    cancelledOn: "",
    cancelledBy: "",
    note: "",
    paymentStatus: false,
    userId: "",
  };
};

export const orderSlice = createSlice({
  name: "order",
  initialState: getInitialOrderState(),
  reducers: {
    setOrderResponse: (state, action) => {
      // Map the server response to the order state
      const {
        id, orderId, orderDate, status, statusUpdatedOn,
        shippingAddress, billingAddress, email, products,
        totalPrice, totalQty, requestToCancel, cancelled,
        cancelledOn, cancelledBy, note, paymentStatus, userId
      } = action.payload;

      state.id = id;
      state.orderId = orderId;
      state.orderDate = orderDate;
      state.status = status;
      state.statusUpdatedOn = statusUpdatedOn;
      state.shippingAddress = shippingAddress;
      state.billingAddress = billingAddress;
      state.email = email;
      state.products = products;
      state.totalPrice = totalPrice;
      state.totalQty = totalQty;
      state.requestToCancel = requestToCancel;
      state.cancelled = cancelled;
      state.cancelledOn = cancelledOn;
      state.cancelledBy = cancelledBy;
      state.note = note;
      state.paymentStatus = paymentStatus;
      state.userId = userId;
    },
    setOrderId: (state, action) => {
      state.id = action.payload.id;
    },
    clearOrder: (state) => {
      return {
        id: "",
        orderId: "",
        orderDate: "",
        status: 0,
        statusUpdatedOn: "",
        shippingAddress: "",
        billingAddress: "",
        email: "",
        products: {},
        totalPrice: 0,
        totalQty: 0,
        requestToCancel: false,
        cancelled: false,
        cancelledOn: "",
        cancelledBy: "",
        note: "",
        paymentStatus: false,
        userId: "",
      };
    },

    resetState: () => {
      // Clear entire Redux state on reset
      localStorage.removeItem("persist:root");
      return {
        id: "",
        orderId: "",
        orderDate: "",
        status: 0,
        statusUpdatedOn: "",
        shippingAddress: "",
        billingAddress: "",
        email: "",
        products: {},
        totalPrice: 0,
        totalQty: 0,
        requestToCancel: false,
        cancelled: false,
        cancelledOn: "",
        cancelledBy: "",
        note: "",
        paymentStatus: false,
        userId: "",
      };
    },
  },
});

export const {
  setOrderId,
  clearOrder,
  resetState,
  setOrderResponse,
} = orderSlice.actions;

export default orderSlice.reducer;

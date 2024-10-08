import React from "react";
import { getAllOrders } from "../../api/services/orderService";
import { useDispatch, useSelector } from "react-redux";
import { ButtonGroup, Button, Badge } from "react-bootstrap";
import { useNavigate } from "react-router-dom";
import { setOrderResponse } from "../../redux/order/orderSlice";
import ViewOrderModal from "./orderModal";

export default function OrderList() {
  const [orderData, setOrderData] = React.useState([]);
  const [loading, setLoading] = React.useState(true);
  const [error, setError] = React.useState(null);
  const [searchQuery, setSearchQuery] = React.useState("");
  const [showModal, setShowModal] = React.useState(false);

  const dispatch = useDispatch();
  const navigate = useNavigate();

  // Get the token from the Redux store
  const token = useSelector((state) => state.auth.loggedUser.token);

  // Enum for order status
  const OrderStatus = {
    0: "Processing",
    1: "Ready",
    2: "Dispatched",
    3: "Delivered",
    4: "Cancelled",
  };

  //handle close modal
  const handleClose = () => {
    setShowModal(false);
  };

  // Function to fetch the list of orders
  const getOrderList = async () => {
    try {
      setLoading(true);
      const response = await getAllOrders(token);
      if (response) {
        setOrderData(response);
        setLoading(false);
        alert("Order List fetched successfully");
        console.log("Order List:", orderData);
      } else {
        setLoading(false);
        alert("Invalid response from server");
        throw new Error("Invalid response from server");
      }
    } catch (error) {
      console.error("Error fetching order list:", error);
      setError("Failed to fetch order list");
      setLoading(Error);
    }
  };

  // Call the getOrderList function when the component loads
  React.useEffect(() => {
    getOrderList();
  }, []);

  // Badge class mapping based on status
  const badgeClass = {
    0: "warning", // Processing
    1: "info", // Ready
    2: "primary", // Dispatched
    3: "success", // Delivered
    4: "danger", // Cancelled
  };

  // Filter orderData based on search query
  const filteredOrders = orderData.filter((order) => {
    const dateParts = order.orderDate.split(" ")[0].split("-"); // Split date into DD-MM-YYYY
    const day = dateParts[0];
    const month = dateParts[1];
    const year = dateParts[2];

    return (
      order.orderId.includes(searchQuery) || // Search by OrderId
      order.email?.toLowerCase().includes(searchQuery.toLowerCase()) || // Search by Email
      day.includes(searchQuery) || // Search by day
      month.includes(searchQuery) || // Search by month
      year.includes(searchQuery) // Search by year
    );
  });

  return (
    <div className="container">
      <div className="title-bar d-flex p-2">
        <input
          type="search"
          className="form-control p-2"
          id="search"
          aria-describedby="search"
          placeholder="Search by Order ID, Date, Month, Year, or Email"
          value={searchQuery}
          onChange={(e) => setSearchQuery(e.target.value)}
        />
     <button type="submit" className="btn btn-primary p-2 m-2 rounded">
          <i className="bi bi-search"></i>
        </button>
      </div>

      <table className="table custom-table">
        <thead>
          <tr>
            <th scope="col">OrderId</th>
            <th scope="col">Date</th>
            <th scope="col">Status</th>
            <th scope="col">Action</th>
          </tr>
        </thead>
        <tbody>
          {filteredOrders?.length > 0 ? (
            filteredOrders.map((order) => (
              <tr key={order.id}>
                <td>{order.orderId}</td>
                <td>{order.orderDate.split(" ")[0]}</td>
                <td>
                  <Badge bg={badgeClass[order.status]}>
                    {OrderStatus[order.status]}
                  </Badge>
                </td>
                <td>
                  <button className="btn btn-primary btn-sm"
                  onClick={() => {
                   dispatch(setOrderResponse(order));  
                   setShowModal(true);   
                  }}
                  >View</button>
                </td>
              </tr>
            ))
          ) : (
            <tr>
              <td colSpan="3">No Orders found found</td>
            </tr>
          )}
        </tbody>
      </table>
      <ViewOrderModal show={showModal} handleClose={handleClose} />       
    </div>
  );
}

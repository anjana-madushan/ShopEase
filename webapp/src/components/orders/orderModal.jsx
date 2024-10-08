/* eslint-disable react/prop-types */
import { Modal, Button, Form, Badge } from "react-bootstrap";
import { useSelector } from "react-redux";
import { useState, useEffect } from "react";
import { getProductByID } from "../../api/services/productService";

export default function ViewOrderModal({ show, handleClose }) {
  // const token = useSelector((state) => state.auth.loggedUser.token);
  const order = useSelector((state) => state.order);

  // Add this to store product details
  const [products, setProducts] = useState({});
  const [searchQuery, setSearchQuery] = useState("");

  useEffect(() => {
    const fetchProducts = async () => {
      const fetchedProducts = {};
      for (const productId of Object.keys(order.products)) {
        const productDetails = await getProductByID(productId);
        fetchedProducts[productId] = productDetails;
      }
      setProducts(fetchedProducts);
    };

    if (order && order.products) {
      fetchProducts();
    }
  }, [order.products]);

  // Filter products based on search query (ignoring case)
  const filteredProducts = Object.entries(order.products).filter(
    ([productId]) =>
      products[productId]?.productName
        ?.toLowerCase()
        .includes(searchQuery.toLowerCase())
  );

  // Enum for order status
  const OrderStatus = {
    0: "Processing",
    1: "Ready",
    2: "Dispatched",
    3: "Delivered",
    4: "Cancelled",
  };

  // Badge class mapping based on status
  const badgeClass = {
    0: "warning", // Processing
    1: "info", // Ready
    2: "primary", // Dispatched
    3: "success", // Delivered
    4: "danger", // Cancelled
  };

  return (
    <Modal show={show} onHide={handleClose} size="lg">
      <Modal.Header closeButton>
        <Modal.Title>Order Details</Modal.Title>
      </Modal.Header>
      <Modal.Body>
        <div className="d-flex p-2 bg-light rounded shadow justify-content-center align-items-center">
          <Form.Group className="m-3">
            <Form.Label className="m-2">Order ID:</Form.Label>
            <Form.Text>{order.orderId}</Form.Text>
          </Form.Group>

          <Form.Group className="m-3">
            <Form.Label className="m-2">OrderedDate:</Form.Label>
            <Form.Text>{order.orderDate.split(" ")[0]}</Form.Text>
          </Form.Group>

          <Form.Group className="m-3">
            <Form.Label className="m-2">Order Status:</Form.Label>
            <Badge bg={badgeClass[order.status]}>
              {OrderStatus[order.status]}
            </Badge>
          </Form.Group>
        </div>
        <div className="d-flex p-2  justify-content-center align-items-center">
          <Form.Group className="m-3">
            <Form.Label className="m-2">Status updated on:</Form.Label>
            <Form.Text>{order.statusUpdatedOn.split(" ")[0]}</Form.Text>
          </Form.Group>

          <Form.Group className="m-3">
            <Form.Label className="m-2">Shipping Address:</Form.Label>
            <Form.Text>{order.shippingAddress}</Form.Text>
          </Form.Group>

          <Form.Group className="m-3">
            <Form.Label className="m-2">Billing Address:</Form.Label>
            <Form.Text>{order.billingAddress}</Form.Text>
          </Form.Group>
        </div>
        <div className="d-flex p-2  justify-content-center align-items-center">
          <Form.Group className="m-3">
            <Form.Label className="m-2">Email:</Form.Label>
            <Form.Text>{order.email}</Form.Text>
          </Form.Group>
        </div>
        <div className="title-bar d-flex p-2">
          <input
            type="search"
            className="form-control p-2"
            id="search"
            aria-describedby="search"
            placeholder="Search by Product Name"
            value={searchQuery}
            onChange={(e) => setSearchQuery(e.target.value)}
          />
        </div>
        <div
          className="card card-body mt-4 overflow-auto"
          style={{ maxHeight: "200px" }}
        >
          {filteredProducts.length === 0 ? (
            <p>No products matching your search.</p>
          ) : (
            <div className="table-responsive w-100">
              <table className="table table-bordered w-100">
                <thead className="thead-light">
                  <tr>
                    <th scope="col">Product Name</th>
                    <th scope="col">Price</th>
                    <th scope="col">Quantity</th>
                  </tr>
                </thead>
                <tbody>
                  {filteredProducts.map(([productId, details]) => (
                    <tr key={productId}>
                      <td>
                        {products[productId]?.productName || "Loading..."}
                      </td>
                      <td>{details.price}</td>
                      <td>{details.quantity}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </div>
        <div className="d-flex p-2  justify-content-center align-items-center">
          <Form.Group className="m-3">
            <Form.Label className="m-2">Total Amount:</Form.Label>
            <Form.Text>{order.totalPrice}</Form.Text>
          </Form.Group>
          <Form.Group className="m-3">
            <Form.Label className="m-2">Total Quantity:</Form.Label>
            <Form.Text>{order.totalQty}</Form.Text>
          </Form.Group>
          <Form.Group className="m-3">
            <Form.Label className="m-2">paymentStatus:</Form.Label>
            <Form.Text>Done!</Form.Text>
          </Form.Group>
        </div>

        {order?.note !== "" && (
          <div className="d-flex p-2  justify-content-center align-items-center">
            <Form.Group className="m-3">
              <Form.Label className="m-2">Note:</Form.Label>
              <Form.Text>{order.note}</Form.Text>
            </Form.Group>
          </div>
        )}
        {order?.requestToCancel == true && (
          <div className="d-flex p-2  justify-content-center align-items-center">
            <button className="btn btn-primary btn-sm m-2 rounded" onClick={handleClose}>
              Approve Cancel Request
            </button>
            <button className="btn btn-danger btn-sm m-2 rounded" onClick={handleClose}>
              Reject Cancel Request
            </button>
          </div>
        )}
        {order?.cancelled == true && (
          <div className="d-flex p-2  justify-content-center align-items-center">
            <Form.Group className="m-3">
              <Form.Label className="m-2">Cancelled on:</Form.Label>
              <Form.Text>{order.cancelledOn}</Form.Text>
            </Form.Group>
            {/* <Form.Group className="m-3">
              <Form.Label className="m-2">Cancelled by:</Form.Label>
              <Form.Text>{order.cancelledBy}</Form.Text>
            </Form.Group> */}
          </div>
        )}
      </Modal.Body>
      <Modal.Footer>
        <Button variant="secondary" onClick={handleClose}>
          Close
        </Button>
      </Modal.Footer>
    </Modal>
  );
}

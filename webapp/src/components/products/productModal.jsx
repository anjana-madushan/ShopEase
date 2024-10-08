/* eslint-disable react/prop-types */
import { useState } from "react";
import { Modal, Button, Form } from "react-bootstrap";


export default function ProductModel({ show, handleClose, handleAddUser }) {
  const [formData, setFormData] = useState({
    productName: "",
    price: "",
    category: "",
    description: "",
    isActive: false,
    stockLevel: "",
    minStockLevel: "",
  });

  const handleChange = (e) => {
    const { name, value, type, checked } = e.target;
    setFormData({
      ...formData,
      [name]: type === "checkbox" ? checked : value,
    });
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    // You can add additional validations if necessary
    handleAddUser(formData); // Call the parent function to handle product creation
    handleClose(); // Close the modal
  };

  return (
    <Modal show={show} onHide={handleClose}>
      <Modal.Header closeButton>
        <Modal.Title>Add New Product</Modal.Title>
      </Modal.Header>
      <Modal.Body>
        <Form onSubmit={handleSubmit}>
          <Form.Group controlId="productName">
            <Form.Label>Product Name</Form.Label>
            <Form.Control
              type="text"
              name="productName"
              value={formData.productName}
              onChange={handleChange}
              required
            />
          </Form.Group>

          <Form.Group controlId="price">
            <Form.Label>Price</Form.Label>
            <Form.Control
              type="number"
              name="price"
              value={formData.price}
              onChange={handleChange}
              required
            />
          </Form.Group>

          <Form.Group controlId="category">
            <Form.Label>Category</Form.Label>
            <Form.Control
              as="select"
              name="category"
              value={formData.category}
              onChange={handleChange}
              required
            >
              <option value="">Select a category</option>
              <option value="kitchen">Kitchen</option>
              <option value="tech">Tech</option>
              <option value="sport">Sport</option>
              <option value="living room">Living Room</option>
              <option value="bathroom">Bathroom</option>
              <option value="other">Other</option>
            </Form.Control>
          </Form.Group>

          <Form.Group controlId="description">
            <Form.Label>Description</Form.Label>
            <Form.Control
              as="textarea"
              name="description"
              value={formData.description}
              onChange={handleChange}
              required
            />
          </Form.Group>

          <Form.Group controlId="isActive">
            <Form.Check
              type="checkbox"
              name="isActive"
              label="Is Active"
              checked={formData.isActive}
              onChange={handleChange}
            />
          </Form.Group>

          <Form.Group controlId="stockLevel">
            <Form.Label>Stock Level</Form.Label>
            <Form.Control
              type="number"
              name="stockLevel"
              value={formData.stockLevel}
              onChange={handleChange}
              required
            />
          </Form.Group>

          <Form.Group controlId="minStockLevel">
            <Form.Label>Minimum Stock Level</Form.Label>
            <Form.Control
              type="number"
              name="minStockLevel"
              value={formData.minStockLevel}
              onChange={handleChange}
              required
            />
          </Form.Group>

          <Button variant="primary" type="submit" className="mt-3">
            Submit
          </Button>
        </Form>
      </Modal.Body>
    </Modal>
  );
}

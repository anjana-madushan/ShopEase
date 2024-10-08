/* eslint-disable react/prop-types */
import { useEffect, useState } from "react";
import { Modal, Button, Form } from "react-bootstrap";
import { getProductByID, updateProductByID } from "../../api/services/productService";
import { useSelector } from "react-redux";


export default function ProductViewModal({ show, handleClose, productId }) {
  const [product, setProduct] = useState({
    productName: '',
    price: '',
    category: '',
    image: null,
    isCategoryActive: false,
    description: '',
    isActive: false,
    stockLevel: 0,
    minStockLevel: 0,
  });
  const [formChanged, setFormChanged] = useState(false);

  const token = useSelector((state) => state.auth.loggedUser.token);

  useEffect(() => {
    if (productId) {
      getProductByID(productId).then((response) => {
        setProduct(response);
      }).catch((error) => {
        console.error("Failed to fetch product details", error);
      });
    }
  }, [productId]);

  const handleInputChange = (e) => {
    const { name, value, type, checked } = e.target;
    setProduct((prevProduct) => ({
      ...prevProduct,
      [name]: type === "checkbox" ? checked : value,
    }));
    setFormChanged(true);
  };

  const handleFormSubmit = (e) => {
    e.preventDefault();
    if (formChanged) {
      const confirmSubmit = window.confirm("Are you sure you want to submit the changes?");
      if (confirmSubmit) {
        updateProductByID(productId, product, token)
          .then(() => {
            alert("Product updated successfully!");
            handleClose();
          })
          .catch((error) => {
            console.error("Failed to update product", error);
            alert("Error updating product");
          });
      }
    }
  };

  return (
    <Modal show={show} onHide={handleClose} size="lg">
      <Modal.Header closeButton>
        <Modal.Title>Edit Product</Modal.Title>
      </Modal.Header>
      <Modal.Body>
        <Form onSubmit={handleFormSubmit}>
          <Form.Group className="mb-3">
            <Form.Label>Product Name</Form.Label>
            <Form.Control
              type="text"
              name="productName"
              value={product.productName}
              onChange={handleInputChange}
              required
            />
          </Form.Group>

          <Form.Group className="mb-3">
            <Form.Label>Price</Form.Label>
            <Form.Control
              type="number"
              name="price"
              value={product.price}
              onChange={handleInputChange}
              required
            />
          </Form.Group>

          <Form.Group className="mb-3">
            <Form.Label>Category</Form.Label>
            <Form.Control
              type="text"
              name="category"
              value={product.category}
              onChange={handleInputChange}
              required
            />
          </Form.Group>

          <Form.Group className="mb-3">
            <Form.Label>Description</Form.Label>
            <Form.Control
              as="textarea"
              name="description"
              value={product.description}
              onChange={handleInputChange}
              rows={3}
            />
          </Form.Group>

          <Form.Group className="mb-3">
            <Form.Label>Stock Level</Form.Label>
            <Form.Control
              type="number"
              name="stockLevel"
              value={product.stockLevel}
              onChange={handleInputChange}
              required
            />
          </Form.Group>

          <Form.Group className="mb-3">
            <Form.Label>Minimum Stock Level</Form.Label>
            <Form.Control
              type="number"
              name="minStockLevel"
              value={product.minStockLevel}
              onChange={handleInputChange}
              required
            />
          </Form.Group>

          <Form.Group className="mb-3" controlId="formCategoryStatus">
            <Form.Check
              type="checkbox"
              name="isCategoryActive"
              checked={product.isCategoryActive}
              onChange={handleInputChange}
              label="Is Category Active?"
              disabled
            />
          </Form.Group>

          <Form.Group className="mb-3" controlId="formProductStatus">
            <Form.Check
              type="checkbox"
              name="isActive"
              checked={product.isActive}
              onChange={handleInputChange}
              label="Is Product Active?"
            />
          </Form.Group>

          <Button variant="primary" type="submit" disabled={!formChanged}>
            Submit
          </Button>
        </Form>
      </Modal.Body>
      <Modal.Footer>
        <Button variant="secondary" onClick={handleClose}>
          Close
        </Button>
      </Modal.Footer>
    </Modal>
  );
}

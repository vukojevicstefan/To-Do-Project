import React, { useState, useEffect, useRef } from "react";
import "../css/AddListModal.css";

const AddListModal = ({ isOpen, onClose, onAdd }) => {
  const [listName, setListName] = useState("");
  const inputRef = useRef(null);

  useEffect(() => {
    if (isOpen && inputRef.current) {
      inputRef.current.focus(); // Focus input when modal opens
    }
  }, [isOpen]);

  const handleAdd = () => {
    if (listName.trim() !== "") {
      onAdd(listName);
      setListName("");
      onClose();
    }
  };

  if (!isOpen) return null;

  return (
    <div className="modal-overlay">
      <div className="modal">
        <h2>Add new list</h2>
        <input
          ref={inputRef}
          type="text"
          placeholder="List name"
          value={listName}
          onChange={(e) => setListName(e.target.value)}
          onKeyDown={(e) => {
            if (e.key === "Enter") handleAdd();
          }}
        />
        <div className="modal-buttons">
          <button className="cancel-button" onClick={onClose}>Cancel</button>
          <button className="add-list-button" onClick={handleAdd}>Add List</button>
        </div>
      </div>
    </div>
  );
};

export default AddListModal;

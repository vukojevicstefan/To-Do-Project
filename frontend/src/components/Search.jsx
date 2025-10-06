import '../css/Search.css';

export default function Search({ toDoLists, setToDoLists }) {
  const handleChange = (e) => {
    const value = e.target.value.toLowerCase();

    // Filter by Name (case-insensitive)
    const filtered = toDoLists.filter(
      (list) =>
        list &&
        typeof list.name === 'string' &&
        list.name.toLowerCase().includes(value)
    );

    setToDoLists(filtered);
  };

  return (
    <input
      type="text"
      placeholder="Search"
      onChange={handleChange}
      className="search-input"
    />
  );
}

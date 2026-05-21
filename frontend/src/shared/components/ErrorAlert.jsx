export function ErrorAlert({ message }) {
  if (!message) return null;

  return (
    <div className="mt-6 p-4 bg-red-900 border border-red-500 text-red-200 rounded-lg max-w-md w-full text-center font-semibold">
      {message}
    </div>
  );
}

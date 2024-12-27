using NotesApiWithUI.Models;

namespace NotesApiWithUI.Services
{
    public class NoteService
    {
        private readonly List<Note> _notes = new();

        public List<Note> GetAll() => _notes;

        public Note? GetById(int id) => _notes.FirstOrDefault(n => n.Id == id);

        public Note Add(Note note)
        {
            note.Id = _notes.Count > 0 ? _notes.Max(n => n.Id) + 1 : 1;
            _notes.Add(note);
            return note;
        }

        public bool Delete(int id)
        {
            var note = GetById(id);
            if (note == null) return false;
            _notes.Remove(note);
            return true;
        }
        public Note? Update(int id, Note updatedNote)
        {
            var note = GetById(id);
            if (note == null) return null;

            note.Title = updatedNote.Title;
            note.Content = updatedNote.Content;
            note.CreatedAt = DateTime.UtcNow; // Обновим дату изменения
            return note;
        }

    }
}

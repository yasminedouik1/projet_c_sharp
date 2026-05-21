let attachedBoards = new WeakSet();

export function attach(board) {
    if (!board || attachedBoards.has(board)) return;
    attachedBoards.add(board);

    board.addEventListener('dragstart', (e) => {
        const card = e.target.closest('.kanban-card');
        if (!card || !e.dataTransfer) return;

        e.dataTransfer.effectAllowed = 'move';
        e.dataTransfer.setDragImage(card, card.offsetWidth / 2, card.offsetHeight / 2);
        card.classList.add('is-dragging');
    });

    board.addEventListener('dragend', (e) => {
        const card = e.target.closest('.kanban-card');
        if (card) card.classList.remove('is-dragging');
    });
}

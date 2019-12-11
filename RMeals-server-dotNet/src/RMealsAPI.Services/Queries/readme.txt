This is the place for queries (repositories).
I'm not using the repository TERM, as it's a pre ORM DDD term.
Within ORM the repository concept does not exists, at least not according the DDD definitions.
We don't have agrregates as well, since the whole object-model graph is accessible any time (at least with lazy load, without due existing navigation properties the boundaries are non-existent again).
The DbContext works as UoW (actually due the silly behaviour of EF the TransactionScope is the one that helps to extend/alter that boundary).

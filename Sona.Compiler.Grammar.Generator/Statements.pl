:- discontiguous block/1.
:- discontiguous has_path/1.
:- discontiguous has_open_path/1.
:- discontiguous has_returning_path/1.
:- discontiguous has_interrupting_path/1.
:- discontiguous not_has_open_path/1.
:- discontiguous not_has_returning_path/1.
:- discontiguous not_has_interrupting_path/1.

% ---------- %
% Categories %
% ---------- %

% free_blocks %
free_blocks(Main, Variadic, Trail) :-
  free_blocks(Main),
  free_blocks_unique(Variadic),
  free_blocks_filter(Main, Variadic),
  special_blocks_ascending(Variadic),
  free_block(Trail).
  
free_blocks_singleton(Main, Variadic, Trail) :- Trail = ignored_block, free_blocks(Main, Variadic, Trail).

free_block(B) :- member(B, [ignored_block, open_block, terminating_block]).
free_blocks([]) :- !.
free_blocks([H]) :- !, free_block(H).
free_blocks([H|T]) :- !, free_blocks([H]), free_blocks(T).
free_blocks_unique(List) :- combinations([open_block, terminating_block], Set), permutation(Set, List).

% Has one non-ignored non-open block.
free_blocks_filter([H|_], _) :- H \= ignored_block, H \= open_block.
free_blocks_filter(_, [H|_]) :- H \= ignored_block, H \= open_block.
free_blocks_filter([_|T], L) :- free_blocks_filter(T, L).
free_blocks_filter(L, [_|T]) :- free_blocks_filter(L, T).

% free_loop_blocks %
free_loop_blocks(Main, Variadic, Trail) :-
  free_loop_blocks(Main),
  free_loop_blocks_unique(Variadic),
  free_loop_blocks_filter(Main, Variadic),
  special_blocks_ascending(Variadic),
  free_block(Trail).
  
free_loop_blocks_singleton(Main, Variadic, Trail) :- Trail = ignored_block, free_loop_blocks(Main, Variadic, Trail).

free_loop_block(B) :- member(B, [ignored_block, open_block, terminating_block, interrupting_block, interruptible_block]).
free_loop_blocks([]) :- !.
free_loop_blocks([H]) :- !, free_loop_block(H).
free_loop_blocks([H|T]) :- !, free_loop_blocks([H]), free_loop_blocks(T).
free_loop_blocks_unique(List) :- combinations([open_block, terminating_block, interrupting_block, interruptible_block], Set), permutation(Set, List).

% Has one non-ignored non-open block.
free_loop_blocks_filter([H|_], _) :- H \= ignored_block, H \= open_block.
free_loop_blocks_filter(_, [H|_]) :- H \= ignored_block, H \= open_block.
free_loop_blocks_filter([_|T], L) :- free_loop_blocks_filter(T, L).
free_loop_blocks_filter(L, [_|T]) :- free_loop_blocks_filter(L, T).

% special_blocks %
special_blocks(Main, Variadic, Trail) :-
  special_blocks(Main),
  special_blocks_unique(Variadic),
  special_blocks_filter(Main, Variadic),
  special_blocks_ascending(Variadic),
  special_block(Trail).

special_block(B) :- member(B, [ignored_block, open_block, terminating_block, interrupting_block, returning_block, interruptible_block, conditional_block]).
special_blocks([]) :- !.
special_blocks([H]) :- !, special_block(H).
special_blocks([H|T]) :- !, special_blocks([H]), special_blocks(T).
special_blocks_unique(List) :- combinations([open_block, terminating_block, interrupting_block, returning_block, interruptible_block, conditional_block], Set), permutation(Set, List).

% Has one non-ignored non-open non-terminating block.
special_blocks_filter([H|_], _) :- H \= ignored_block, H \= open_block, H \= terminating_block.
special_blocks_filter(_, [H|_]) :- H \= ignored_block, H \= open_block, H \= terminating_block.
special_blocks_filter([_|T], L) :- special_blocks_filter(T, L).
special_blocks_filter(L, [_|T]) :- special_blocks_filter(L, T).

% Each consecutive block adds a path and does not remove a path.
special_blocks_ascending([]) :- !.
special_blocks_ascending([_]) :- !.
special_blocks_ascending([A|[B|T]]) :- !, adds_path(A, B), !, not(adds_path(B, A)), !, special_blocks_ascending([B|T]).

combinations([], []).
combinations([_|T1], T2) :- combinations(T1, T2).
combinations([H|T1], [H|T2]) :- combinations(T1, T2).

adds_path(A, B) :- has_open_path(B), not_has_open_path(A).
adds_path(A, B) :- has_interrupting_path(B), not_has_interrupting_path(A).
adds_path(A, B) :- has_returning_path(B), not_has_returning_path(A).

% ignored_block %
block(ignored_block).

% open_block %
block(open_block).
has_path(open_block).
has_open_path(open_block).
not_has_returning_path(open_block).
not_has_interrupting_path(open_block).

% terminating_block %
block(terminating_block).
has_path(terminating_block).
not_has_open_path(terminating_block).
not_has_returning_path(terminating_block).
not_has_interrupting_path(terminating_block).

% interrupting_block %
block(interrupting_block).
has_path(interrupting_block).
has_interrupting_path(interrupting_block).
not_has_open_path(interrupting_block).
not_has_returning_path(interrupting_block).

% returning_block %
block(returning_block).
has_path(returning_block).
has_returning_path(returning_block).
has_interrupting_path(returning_block).
not_has_open_path(returning_block).

% interruptible_block %
block(interruptible_block).
has_path(interruptible_block).
has_open_path(interruptible_block).
has_interrupting_path(interruptible_block).
not_has_returning_path(interruptible_block).

% conditional_block %
block(conditional_block).
has_path(conditional_block).
has_open_path(conditional_block).
has_returning_path(conditional_block).
has_interrupting_path(conditional_block).

% Simplified predicates
not_has_open_path(Statement) :- !, not(has_open_path(Statement)).
not_has_returning_path(Statement) :- !, not(has_returning_path(Statement)).
not_has_interrupting_path(Statement) :- !, not(has_interrupting_path(Statement)).
has_any_path([[H|T]]) :- !, has_any_path([H|T]).
has_any_path([H|_]) :- has_path(H).
has_any_path([_|T]) :- !, has_any_path(T).
has_any_open_path([[H|T]]) :- !, has_any_open_path([H|T]).
has_any_open_path([H|_]) :- has_open_path(H).
has_any_open_path([_|T]) :- !, has_any_open_path(T).
has_any_returning_path([[H|T]]) :- !, has_any_returning_path([H|T]).
has_any_returning_path([H|_]) :- has_returning_path(H).
has_any_returning_path([_|T]) :- !, has_any_returning_path(T).
has_any_interrupting_path([[H|T]]) :- !, has_any_interrupting_path([H|T]).
has_any_interrupting_path([H|_]) :- has_interrupting_path(H).
has_any_interrupting_path([_|T]) :- !, has_any_interrupting_path(T).
has_all_path([]).
has_all_path([A|T]) :- !, has_path(A), !, has_all_path(T).

% ------------------ %
% Derived predicates %
% ------------------ %

is_terminating(Statement) :- !, has_path(Statement), !, not_has_open_path(Statement), !, not_has_returning_path(Statement), !, not_has_interrupting_path(Statement).
is_interrupting(Statement) :- !, has_interrupting_path(Statement), !, not_has_open_path(Statement), !, not_has_returning_path(Statement).
is_interruptible(Statement) :- !, has_interrupting_path(Statement), !, has_open_path(Statement), !, not_has_returning_path(Statement).
is_returning(Statement) :- !, has_returning_path(Statement), !, not_has_open_path(Statement).
is_conditional(Statement) :- !, has_returning_path(Statement), !, has_open_path(Statement).

% ------------ %
% do_statement %
% ------------ %

% If trail is ignored, there must be no open path.
has_path(do_statement(Block, ignored_block)) :- !, has_path(Block), not_has_open_path(Block).

% If trail is accessible, there must be an open path to it.
has_path(do_statement(Block, Trail)) :- !, has_path(Trail), has_open_path(Block).

% If trail is open, it must be accessible.
has_open_path(do_statement(Block, Trail)) :- !, has_open_path(Trail), !, has_path(do_statement(Block, Trail)).

% Check trail and find in any part.
has_returning_path(do_statement(Block, Trail)) :- !, has_path(do_statement(Block, Trail)), has_any_returning_path([Trail, Block]).
has_interrupting_path(do_statement(Block, Trail)) :- !, has_path(do_statement(Block, Trail)), has_any_interrupting_path([Trail, Block]).

% --------------- %
% while_statement %
% --------------- %

% The trail is always accessible.
has_path(while_statement(Block, Trail)) :- !, has_all_path([Trail, Block]).

% If trail is open, it must be accessible.
has_open_path(while_statement(Block, Trail)) :- !, has_open_path(Trail), !, has_path(while_statement(Block, Trail)).

% Check trail and find in any part.
has_returning_path(while_statement(Block, Trail)) :- !, has_path(while_statement(Block, Trail)), has_any_returning_path([Trail, Block]).

% Inner interrupting statements are irrelevant.
has_interrupting_path(while_statement(Block, Trail)) :- !, has_path(while_statement(Block, Trail)), has_interrupting_path(Trail).

% -------------------- %
% while_true_statement %
% -------------------- %

% If trail is ignored, there must be no open path.
has_path(while_true_statement(Block, ignored_block)) :- !, has_path(Block), not_has_open_path(Block).

% If trail is accessible, there must be an open path to it.
has_path(while_true_statement(Block, Trail)) :- !, has_path(Trail), has_open_path(Block).

% If trail is open, it must be accessible.
has_open_path(while_true_statement(Block, Trail)) :- !, has_open_path(Trail), !, has_path(while_true_statement(Block, Trail)).

% Check trail and find in any part.
has_returning_path(while_true_statement(Block, Trail)) :- !, has_path(while_true_statement(Block, Trail)), has_any_returning_path([Trail, Block]).

% Inner interrupting statements are irrelevant.
has_interrupting_path(while_true_statement(Block, Trail)) :- !, has_path(while_true_statement(Block, Trail)), has_interrupting_path(Trail).

% ------------ %
% if_statement %
% ------------ %

% If trail is ignored, there must be no open path.
has_path(if_statement(Then, Elseifs, Else, ignored_block)) :- !, has_all_path([Then, Else]), has_all_path(Elseifs), not_has_open_path(Then), not_has_open_path(Else), not(has_any_open_path(Elseifs)).

% If trail is accessible, there must be an open path to it.
has_path(if_statement(Then, Elseifs, ignored_block, Trail)) :- !, has_all_path([Trail, Then]), has_all_path(Elseifs).
has_path(if_statement(Then, Elseifs, Else, Trail)) :- !, has_all_path([Trail, Then, Else]), has_all_path(Elseifs), has_any_open_path([Then, Else, Elseifs]).

% If trail is open, it must be accessible.
has_open_path(if_statement(Then, Elseifs, Else, Trail)) :- !, has_open_path(Trail), !, has_path(if_statement(Then, Elseifs, Else, Trail)).

% Check trail and find in any part.
has_returning_path(if_statement(Then, Elseifs, Else, Trail)) :- !, has_path(if_statement(Then, Elseifs, Else, Trail)), has_any_returning_path([Trail, Then, Else, Elseifs]).
has_interrupting_path(if_statement(Then, Elseifs, Else, Trail)) :- !, has_path(if_statement(Then, Elseifs, Else, Trail)), has_any_interrupting_path([Trail, Then, Else, Elseifs]).

% --------------------- %
% if_statement_no_trail %
% --------------------- %

% Trail must be ignored.
has_path(if_statement_no_trail(Then, Elseifs, Else, ignored_block)) :- !, has_path(if_statement(Then, Elseifs, Else, ignored_block)).
has_open_path(if_statement_no_trail(Then, Elseifs, Else, ignored_block)) :- !, has_open_path(if_statement(Then, Elseifs, Else, ignored_block)).
has_returning_path(if_statement_no_trail(Then, Elseifs, Else, ignored_block)) :- !, has_returning_path(if_statement(Then, Elseifs, Else, ignored_block)).
has_interrupting_path(if_statement_no_trail(Then, Elseifs, Else, ignored_block)) :- !, has_interrupting_path(if_statement(Then, Elseifs, Else, ignored_block)).

% ------------------ %
% if_statement_trail %
% ------------------ %

% Trail must be accessible.
has_path(if_statement_trail(Then, Elseifs, Else, Trail)) :- !, has_path(Trail), has_path(if_statement(Then, Elseifs, Else, Trail)).
has_open_path(if_statement_trail(Then, Elseifs, Else, Trail)) :- !, has_path(Trail), has_open_path(if_statement(Then, Elseifs, Else, Trail)).
has_returning_path(if_statement_trail(Then, Elseifs, Else, Trail)) :- !, has_path(Trail), has_returning_path(if_statement(Then, Elseifs, Else, Trail)).
has_interrupting_path(if_statement_trail(Then, Elseifs, Else, Trail)) :- !, has_path(Trail), has_interrupting_path(if_statement(Then, Elseifs, Else, Trail)).

% ---------------------------- %
% if_statement_trail_from_else %
% ---------------------------- %

% Trail must be accessible and must be semantically equivalent to putting it in Else.
has_path(if_statement_trail_from_else(Then, Elseifs, Else, Trail)) :- !, has_path(Trail), has_open_path(Else), not_has_open_path(Then), not(has_any_open_path(Elseifs)), !, has_path(if_statement(Then, Elseifs, Else, Trail)).
has_open_path(if_statement_trail_from_else(Then, Elseifs, Else, Trail)) :- !, has_path(if_statement_trail_from_else(Then, Elseifs, Else, Trail)), has_open_path(if_statement(Then, Elseifs, Else, Trail)).
has_returning_path(if_statement_trail_from_else(Then, Elseifs, Else, Trail)) :- !, has_path(if_statement_trail_from_else(Then, Elseifs, Else, Trail)), has_returning_path(if_statement(Then, Elseifs, Else, Trail)).
has_interrupting_path(if_statement_trail_from_else(Then, Elseifs, Else, Trail)) :- !, has_path(if_statement_trail_from_else(Then, Elseifs, Else, Trail)), has_interrupting_path(if_statement(Then, Elseifs, Else, Trail)).

% ---------------- %
% switch_statement %
% ---------------- %

% The cases are always exhaustive.

% If trail is ignored, there must be no open path.
has_path(switch_statement(Cases, ignored_block, ignored_block)) :- !, has_all_path(Cases), not(has_any_open_path(Cases)).
has_path(switch_statement(Cases, Else, ignored_block)) :- !, has_path(Else), has_all_path(Cases), not_has_open_path(Else), not(has_any_open_path(Cases)).

% If trail is accessible, there must be an open path to it.
has_path(switch_statement(Cases, ignored_block, Trail)) :- !, has_path(Trail), has_all_path(Cases), has_any_open_path(Cases).
has_path(switch_statement(Cases, Else, Trail)) :- !, has_all_path([Trail, Else]), has_all_path(Cases), has_any_open_path([Else, Cases]).

% If trail is open, it must be accessible.
has_open_path(switch_statement(Cases, Else, Trail)) :- !, has_open_path(Trail), !, has_path(switch_statement(Cases, Else, Trail)).

% Check trail and find in any part.
has_returning_path(switch_statement(Cases, Else, Trail)) :- !, has_path(switch_statement(Cases, Else, Trail)), has_any_returning_path([Trail, Else, Cases]).

% Inner interrupting statements are irrelevant.
has_interrupting_path(switch_statement(Cases, Else, Trail)) :- !, has_path(switch_statement(Cases, Else, Trail)), has_interrupting_path(Trail).

% ------------------------- %
% switch_statement_no_trail %
% ------------------------- %

% Trail must be ignored.
has_path(switch_statement_no_trail(Cases, Else, ignored_block)) :- !, has_path(switch_statement(Cases, Else, ignored_block)).
has_open_path(switch_statement_no_trail(Cases, Else, ignored_block)) :- !, has_open_path(switch_statement(Cases, Else, ignored_block)).
has_returning_path(switch_statement_no_trail(Cases, Else, ignored_block)) :- !, has_returning_path(switch_statement(Cases, Else, ignored_block)).
has_interrupting_path(switch_statement_no_trail(Cases, Else, ignored_block)) :- !, has_interrupting_path(switch_statement(Cases, Else, ignored_block)).

% ---------------------- %
% switch_statement_trail %
% ---------------------- %

% Trail must be accessible.
has_path(switch_statement_trail(Cases, Else, Trail)) :- !, has_path(Trail), has_path(switch_statement(Cases, Else, Trail)).
has_open_path(switch_statement_trail(Cases, Else, Trail)) :- !, has_path(Trail), has_open_path(switch_statement(Cases, Else, Trail)).
has_returning_path(switch_statement_trail(Cases, Else, Trail)) :- !, has_path(Trail), has_returning_path(switch_statement(Cases, Else, Trail)).
has_interrupting_path(switch_statement_trail(Cases, Else, Trail)) :- !, has_path(Trail), has_interrupting_path(switch_statement(Cases, Else, Trail)).

% ----------------------- %
% switch_statement_simple %
% ----------------------- %

% Branches have no interrupting path.
has_path(switch_statement_simple(Cases, Else, Trail)) :- !, not(has_any_interrupting_path([Else, Cases])), has_path(switch_statement(Cases, Else, Trail)).
has_open_path(switch_statement_simple(Cases, Else, Trail)) :- !, has_path(switch_statement_simple(Cases, Else, Trail)), has_open_path(switch_statement(Cases, Else, Trail)).
has_returning_path(switch_statement_simple(Cases, Else, Trail)) :- !, has_path(switch_statement_simple(Cases, Else, Trail)), has_returning_path(switch_statement(Cases, Else, Trail)).
has_interrupting_path(switch_statement_trail(Cases, Else, Trail)) :- !, has_path(switch_statement_simple(Cases, Else, Trail)), has_interrupting_path(switch_statement(Cases, Else, Trail)).

% ---------------------------- %
% switch_statement_interrupted %
% ---------------------------- %

% Branches contain an interrupting path.
has_path(switch_statement_interrupted(Cases, Else, Trail)) :- !, has_any_interrupting_path([Else, Cases]), has_path(switch_statement(Cases, Else, Trail)).
has_open_path(switch_statement_interrupted(Cases, Else, Trail)) :- !, has_path(switch_statement_interrupted(Cases, Else, Trail)), has_open_path(switch_statement(Cases, Else, Trail)).
has_returning_path(switch_statement_interrupted(Cases, Else, Trail)) :- !, has_path(switch_statement_interrupted(Cases, Else, Trail)), has_returning_path(switch_statement(Cases, Else, Trail)).
has_interrupting_path(switch_statement_interrupted(Cases, Else, Trail)) :- !, has_path(switch_statement_interrupted(Cases, Else, Trail)), has_interrupting_path(switch_statement(Cases, Else, Trail)).

% -------------------------------- %
% switch_statement_no_trail_simple %
% -------------------------------- %

has_path(switch_statement_no_trail_simple(Cases, Else, Trail)) :- !, has_path(switch_statement_no_trail(Cases, Else, Trail)), has_path(switch_statement_simple(Cases, Else, Trail)).
has_open_path(switch_statement_no_trail_simple(Cases, Else, Trail)) :- !, has_open_path(switch_statement_no_trail(Cases, Else, Trail)), has_open_path(switch_statement_simple(Cases, Else, Trail)).
has_returning_path(switch_statement_no_trail_simple(Cases, Else, Trail)) :- !, has_returning_path(switch_statement_no_trail(Cases, Else, Trail)), has_returning_path(switch_statement_simple(Cases, Else, Trail)).
has_interrupting_path(switch_statement_no_trail_simple(Cases, Else, Trail)) :- !, has_interrupting_path(switch_statement_no_trail(Cases, Else, Trail)), has_interrupting_path(switch_statement_simple(Cases, Else, Trail)).

% ------------------------------------- %
% switch_statement_no_trail_interrupted %
% ------------------------------------- %

has_path(switch_statement_no_trail_interrupted(Cases, Else, Trail)) :- !, has_path(switch_statement_no_trail(Cases, Else, Trail)), has_path(switch_statement_interrupted(Cases, Else, Trail)).
has_open_path(switch_statement_no_trail_interrupted(Cases, Else, Trail)) :- !, has_open_path(switch_statement_no_trail(Cases, Else, Trail)), has_open_path(switch_statement_interrupted(Cases, Else, Trail)).
has_returning_path(switch_statement_no_trail_interrupted(Cases, Else, Trail)) :- !, has_returning_path(switch_statement_no_trail(Cases, Else, Trail)), has_returning_path(switch_statement_interrupted(Cases, Else, Trail)).
has_interrupting_path(switch_statement_no_trail_interrupted(Cases, Else, Trail)) :- !, has_interrupting_path(switch_statement_no_trail(Cases, Else, Trail)), has_interrupting_path(switch_statement_interrupted(Cases, Else, Trail)).

% -------------------------------- %
% switch_statement_trail_simple %
% -------------------------------- %

has_path(switch_statement_trail_simple(Cases, Else, Trail)) :- !, has_path(switch_statement_trail(Cases, Else, Trail)), has_path(switch_statement_simple(Cases, Else, Trail)).
has_open_path(switch_statement_trail_simple(Cases, Else, Trail)) :- !, has_open_path(switch_statement_trail(Cases, Else, Trail)), has_open_path(switch_statement_simple(Cases, Else, Trail)).
has_returning_path(switch_statement_trail_simple(Cases, Else, Trail)) :- !, has_returning_path(switch_statement_trail(Cases, Else, Trail)), has_returning_path(switch_statement_simple(Cases, Else, Trail)).
has_interrupting_path(switch_statement_trail_simple(Cases, Else, Trail)) :- !, has_interrupting_path(switch_statement_trail(Cases, Else, Trail)), has_interrupting_path(switch_statement_simple(Cases, Else, Trail)).

% ------------------------------------- %
% switch_statement_trail_interrupted %
% ------------------------------------- %

has_path(switch_statement_trail_interrupted(Cases, Else, Trail)) :- !, has_path(switch_statement_trail(Cases, Else, Trail)), has_path(switch_statement_interrupted(Cases, Else, Trail)).
has_open_path(switch_statement_trail_interrupted(Cases, Else, Trail)) :- !, has_open_path(switch_statement_trail(Cases, Else, Trail)), has_open_path(switch_statement_interrupted(Cases, Else, Trail)).
has_returning_path(switch_statement_trail_interrupted(Cases, Else, Trail)) :- !, has_returning_path(switch_statement_trail(Cases, Else, Trail)), has_returning_path(switch_statement_interrupted(Cases, Else, Trail)).
has_interrupting_path(switch_statement_trail_interrupted(Cases, Else, Trail)) :- !, has_interrupting_path(switch_statement_trail(Cases, Else, Trail)), has_interrupting_path(switch_statement_interrupted(Cases, Else, Trail)).



% --------- %
% Execution %
% --------- %

:- use_module(library(http/json)).

save(Filename, Term) :-
  open(Filename, write, Stream),
  json_write(Stream, Term, [serialize_unknown(true)]),
  close(Stream).

analyze_do(Blocks, Predicate, Result) :-
  setof(result{body:Body,trail:Trail}, (call(Blocks, [Body], [], Trail), call(Predicate, do_statement(Body, Trail))), Result).

analyze_while(Blocks, Predicate, Result) :-
  setof(result{body:Body,trail:Trail}, (call(Blocks, [Body], [], Trail), call(Predicate, while_statement(Body, Trail))), Result).

analyze_while_true(Blocks, Predicate, Result) :-
  setof(result{body:Body,trail:Trail}, (call(Blocks, [Body], [], Trail), call(Predicate, while_true_statement(Body, Trail))), Result).

analyze_if(Blocks, Predicate, Result) :-
  setof(result{then:Then,elseif:Elseifs,else:Else,trail:Trail}, (call(Blocks, [Then, Else], Elseifs, Trail), call(Predicate, if_statement(Then, Elseifs, Else, Trail))), Result).

analyze_if_no_trail(Blocks, Predicate, Result) :-
  setof(result{then:Then,elseif:Elseifs,else:Else,trail:Trail}, (call(Blocks, [Then, Else], Elseifs, Trail), call(Predicate, if_statement_no_trail(Then, Elseifs, Else, Trail))), Result).

analyze_if_trail(Blocks, Predicate, Result) :-
  setof(result{then:Then,elseif:Elseifs,else:Else,trail:Trail}, (call(Blocks, [Then, Else], Elseifs, Trail), call(Predicate, if_statement_trail(Then, Elseifs, Else, Trail))), Result).

analyze_if_trail_from_else(Blocks, Predicate, Result) :-
  setof(result{then:Then,elseif:Elseifs,else:Else,trail:Trail}, (call(Blocks, [Then, Else], Elseifs, Trail), call(Predicate, if_statement_trail_from_else(Then, Elseifs, Else, Trail))), Result).

analyze_switch(Blocks, Predicate, Result) :-
  setof(result{case:Cases,else:Else,trail:Trail}, (call(Blocks, [Else], Cases, Trail), call(Predicate, switch_statement(Cases, Else, Trail))), Result).

analyze_switch_simple(Blocks, Predicate, Result) :-
  setof(result{case:Cases,else:Else,trail:Trail}, (call(Blocks, [Else], Cases, Trail), call(Predicate, switch_statement_simple(Cases, Else, Trail))), Result).

analyze_switch_interrupted(Blocks, Predicate, Result) :-
  setof(result{case:Cases,else:Else,trail:Trail}, (call(Blocks, [Else], Cases, Trail), call(Predicate, switch_statement_interrupted(Cases, Else, Trail))), Result).

analyze_switch_no_trail_simple(Blocks, Predicate, Result) :-
  setof(result{case:Cases,else:Else,trail:Trail}, (call(Blocks, [Else], Cases, Trail), call(Predicate, switch_statement_no_trail_simple(Cases, Else, Trail))), Result).

analyze_switch_trail_simple(Blocks, Predicate, Result) :-
  setof(result{case:Cases,else:Else,trail:Trail}, (call(Blocks, [Else], Cases, Trail), call(Predicate, switch_statement_trail_simple(Cases, Else, Trail))), Result).

analyze_switch_no_trail_interrupted(Blocks, Predicate, Result) :-
  setof(result{case:Cases,else:Else,trail:Trail}, (call(Blocks, [Else], Cases, Trail), call(Predicate, switch_statement_no_trail_interrupted(Cases, Else, Trail))), Result).

analyze_switch_trail_interrupted(Blocks, Predicate, Result) :-
  setof(result{case:Cases,else:Else,trail:Trail}, (call(Blocks, [Else], Cases, Trail), call(Predicate, switch_statement_trail_interrupted(Cases, Else, Trail))), Result).

analyze_singleton(Analyzer, Predicate, Result) :- call(Analyzer, free_blocks_singleton, Predicate, Result) ; Result = [].
analyze_loop_singleton(Analyzer, Predicate, Result) :- call(Analyzer, free_loop_blocks_singleton, Predicate, Result) ; Result = [].
analyze_trailed(Analyzer, Predicate, Result) :- call(Analyzer, special_blocks, Predicate, Result) ; Result = [].

run :-
  analyze_singleton(analyze_do, is_terminating, DoTerminating),
  analyze_trailed(analyze_do, is_interrupting, DoInterrupting),
  analyze_trailed(analyze_do, is_returning, DoReturning),
  analyze_trailed(analyze_do, is_interruptible, DoInterruptible),
  analyze_trailed(analyze_do, is_conditional, DoConditional),
  save('grammar_do.json', result{terminating:DoTerminating, interrupting:DoInterrupting, returning:DoReturning, interruptible:DoInterruptible, conditional:DoConditional}),
  
  analyze_trailed(analyze_do, is_interrupting, WhileInterrupting),
  analyze_trailed(analyze_while, is_returning, WhileReturning),
  analyze_trailed(analyze_while, is_interruptible, WhileInterruptible),
  analyze_trailed(analyze_while, is_conditional, WhileConditional),
  save('grammar_while.json', result{interrupting:WhileInterrupting, returning:WhileReturning, interruptible:WhileInterruptible, conditional:WhileConditional}),
  
  analyze_singleton(analyze_while_true, is_terminating, WhileTrueTerminating),
  analyze_trailed(analyze_while_true, is_interrupting, WhileTrueInterrupting),
  analyze_trailed(analyze_while_true, is_returning, WhileTrueReturning),
  analyze_trailed(analyze_while_true, is_interruptible, WhileTrueInterruptible),
  analyze_trailed(analyze_while_true, is_conditional, WhileTrueConditional),
  save('grammar_while_true.json', result{terminating:WhileTrueTerminating, interrupting:WhileTrueInterrupting, returning:WhileTrueReturning, interruptible:WhileTrueInterruptible, conditional:WhileTrueConditional}),
  
  analyze_loop_singleton(analyze_switch_simple, is_terminating, SwitchTerminatingSimple),
  analyze_trailed(analyze_switch_simple, is_interruptible, SwitchInterruptibleSimple),
  analyze_trailed(analyze_switch_simple, is_conditional, SwitchConditionalSimple),
  save('grammar_switch_simple.json', result{terminating:SwitchTerminatingSimple, interruptible:SwitchInterruptibleSimple, conditional:SwitchConditionalSimple}),
  
  analyze_loop_singleton(analyze_switch_interrupted, is_terminating, SwitchTerminatingInterrupted),
  analyze_trailed(analyze_switch_interrupted, is_interruptible, SwitchInterruptibleInterrupted),
  analyze_trailed(analyze_switch_interrupted, is_conditional, SwitchConditionalInterrupted),
  save('grammar_switch_interrupted.json', result{terminating:SwitchTerminatingInterrupted, interruptible:SwitchInterruptibleInterrupted, conditional:SwitchConditionalInterrupted}),
  
  analyze_trailed(analyze_switch_no_trail_simple, is_interrupting, SwitchInterruptingNoTrailSimple),
  analyze_trailed(analyze_switch_no_trail_simple, is_returning, SwitchReturningNoTrailSimple),
  save('grammar_switch_no_trail_simple.json', result{interrupting:SwitchInterruptingNoTrailSimple, returning:SwitchReturningNoTrailSimple}),
  
  analyze_trailed(analyze_switch_trail_simple, is_interrupting, SwitchInterruptingTrailSimple),
  analyze_trailed(analyze_switch_trail_simple, is_returning, SwitchReturningTrailSimple),
  save('grammar_switch_trail_simple.json', result{interrupting:SwitchInterruptingTrailSimple, returning:SwitchReturningTrailSimple}),
  
  analyze_trailed(analyze_switch_no_trail_interrupted, is_interrupting, SwitchInterruptingNoTrailInterrupted),
  analyze_trailed(analyze_switch_no_trail_interrupted, is_returning, SwitchReturningNoTrailInterrupted),
  save('grammar_switch_no_trail_interrupted.json', result{interrupting:SwitchInterruptingNoTrailInterrupted, returning:SwitchReturningNoTrailInterrupted}),
  
  analyze_trailed(analyze_switch_trail_interrupted, is_interrupting, SwitchInterruptingTrailInterrupted),
  analyze_trailed(analyze_switch_trail_interrupted, is_returning, SwitchReturningTrailInterrupted),
  save('grammar_switch_trail_interrupted.json', result{interrupting:SwitchInterruptingTrailInterrupted, returning:SwitchReturningTrailInterrupted}),
  
  analyze_singleton(analyze_if, is_terminating, IfTerminating),
  analyze_trailed(analyze_if, is_interruptible, IfInterruptible),
  analyze_trailed(analyze_if, is_conditional, IfConditional),
  save('grammar_if.json', result{terminating:IfTerminating, interruptible:IfInterruptible, conditional:IfConditional}),
  
  analyze_trailed(analyze_if_no_trail, is_interrupting, IfInterruptingNoTrail),
  analyze_trailed(analyze_if_no_trail, is_returning, IfReturningNoTrail),
  save('grammar_if_no_trail.json', result{interrupting:IfInterruptingNoTrail, returning:IfReturningNoTrail}),
  
  analyze_trailed(analyze_if_trail, is_interrupting, IfInterruptingTrail),
  analyze_trailed(analyze_if_trail, is_returning, IfReturningTrail),  
  save('grammar_if_trail.json', result{interrupting:IfInterruptingTrail, returning:IfReturningTrail}),
  
  analyze_trailed(analyze_if_trail_from_else, is_returning, IfReturningTrailFromElse),  
  save('grammar_if_trail_from_else.json', result{returning:IfReturningTrailFromElse}).

:- discontiguous block/1.
:- discontiguous has_path/1.
:- discontiguous has_open_path/1.
:- discontiguous has_returning_path/1.
:- discontiguous not_has_open_path/1.
:- discontiguous not_has_returning_path/1.

% ---------- %
% Categories %
% ---------- %

% free_blocks %
free_blocks(Main, Variadic, Trail) :-
  free_blocks(Main),
  free_blocks_unique(Variadic),
  free_blocks_filter(Main, Variadic),
  free_block(Trail).
  
free_blocks_singleton(Main, Variadic, Trail) :- Trail = ignored_block, free_blocks(Main, Variadic, Trail).

free_block(B) :- member(B, [ignored_block, open_block, terminating_block]).
free_blocks([]) :- !.
free_blocks([H]) :- !, free_block(H).
free_blocks([H|T]) :- !, free_blocks([H]), free_blocks(T).
free_blocks_unique(List) :- combinations([open_block, terminating_block], Set), permutation(Set, List).
free_blocks_filter([H|_], _) :- H \= ignored_block, H \= open_block.
free_blocks_filter(_, [H|_]) :- H \= ignored_block, H \= open_block.
free_blocks_filter([_|T], L) :- free_blocks_filter(T, L).
free_blocks_filter(L, [_|T]) :- free_blocks_filter(L, T).

% special_blocks %
special_blocks(Main, Variadic, Trail) :-
  special_blocks(Main),
  special_blocks_unique(Variadic),
  special_blocks_filter(Main, Variadic),
  special_block(Trail).

special_block(B) :- member(B, [ignored_block, open_block, terminating_block, returning_block, conditional_block]).
special_blocks([]) :- !.
special_blocks([H]) :- !, special_block(H).
special_blocks([H|T]) :- !, special_blocks([H]), special_blocks(T).
special_blocks_unique(List) :- combinations([open_block, terminating_block, returning_block, conditional_block], Set), permutation(Set, List).
special_blocks_filter([H|_], _) :- H \= ignored_block, H \= open_block, H \= terminating_block.
special_blocks_filter(_, [H|_]) :- H \= ignored_block, H \= open_block, H \= terminating_block.
special_blocks_filter([_|T], L) :- special_blocks_filter(T, L).
special_blocks_filter(L, [_|T]) :- special_blocks_filter(L, T).

combinations([], []).
combinations([_|T1], T2) :- combinations(T1, T2).
combinations([H|T1], [H|T2]) :- combinations(T1, T2).

% ignored_block %
block(ignored_block).

% open_block %
block(open_block).
has_path(open_block).
has_open_path(open_block).
not_has_returning_path(open_block).

% terminating_block %
block(terminating_block).
has_path(terminating_block).
not_has_open_path(terminating_block).
not_has_returning_path(terminating_block).

% returning_block %
block(returning_block).
has_path(returning_block).
has_returning_path(returning_block).
not_has_open_path(returning_block).

% conditional_block %
block(conditional_block).
has_path(conditional_block).
has_open_path(conditional_block).
has_returning_path(conditional_block).

% Simplified predicates
not_has_open_path(Statement) :- !, not(has_open_path(Statement)).
not_has_returning_path(Statement) :- !, not(has_returning_path(Statement)).
has_any_path([[H|T]]) :- !, has_any_path([H|T]).
has_any_path([H|_]) :- has_path(H).
has_any_path([_|T]) :- !, has_any_path(T).
has_any_open_path([[H|T]]) :- !, has_any_open_path([H|T]).
has_any_open_path([H|_]) :- has_open_path(H).
has_any_open_path([_|T]) :- !, has_any_open_path(T).
has_any_returning_path([[H|T]]) :- !, has_any_returning_path([H|T]).
has_any_returning_path([H|_]) :- has_returning_path(H).
has_any_returning_path([_|T]) :- !, has_any_returning_path(T).
has_all_path([]).
has_all_path([A|T]) :- !, has_path(A), !, has_all_path(T).

% ------------------ %
% Derived predicates %
% ------------------ %

is_terminating(Statement) :- !, has_path(Statement), !, not_has_open_path(Statement), !, not_has_returning_path(Statement).
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

% --------------- %
% while_statement %
% --------------- %

% The trail is always accessible.
has_path(while_statement(Block, Trail)) :- !, has_all_path([Trail, Block]).

% If trail is open, it must be accessible.
has_open_path(while_statement(Block, Trail)) :- !, has_open_path(Trail), !, has_path(while_statement(Block, Trail)).

% Check trail and find in any part.
has_returning_path(while_statement(Block, Trail)) :- !, has_path(while_statement(Block, Trail)), has_any_returning_path([Trail, Block]).

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

analyze_singleton(Analyzer, Predicate, Result) :- call(Analyzer, free_blocks_singleton, Predicate, Result).
analyze_trailed(Analyzer, Predicate, Result) :- call(Analyzer, special_blocks, Predicate, Result).

run :-
  analyze_singleton(analyze_do, is_terminating, DoTerminating),
  analyze_trailed(analyze_do, is_returning, DoReturning),
  analyze_trailed(analyze_do, is_conditional, DoConditional),
  save('grammar_do.json', result{terminating:DoTerminating, returning:DoReturning, conditional:DoConditional}),
  
  analyze_trailed(analyze_while, is_returning, WhileReturning),
  analyze_trailed(analyze_while, is_conditional, WhileConditional),
  save('grammar_while.json', result{returning:WhileReturning, conditional:WhileConditional}),
  
  analyze_singleton(analyze_while_true, is_terminating, WhileTrueTerminating),
  analyze_trailed(analyze_while_true, is_returning, WhileTrueReturning),
  analyze_trailed(analyze_while_true, is_conditional, WhileTrueConditional),
  save('grammar_while_true.json', result{terminating:WhileTrueTerminating, returning:WhileTrueReturning, conditional:WhileTrueConditional}),
  
  analyze_singleton(analyze_if, is_terminating, IfTerminating),
  analyze_trailed(analyze_if, is_returning, IfReturning),
  analyze_trailed(analyze_if, is_conditional, IfConditional),
  save('grammar_if.json', result{terminating:IfTerminating, returning:IfReturning, conditional:IfConditional}).

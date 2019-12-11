$(document).on('click', '[data-toggle="class"]', function () {
  var $target = $($(this).data('target'));

  var toggleClasses = $(this).data('toggle-classes');
  if (toggleClasses)
    $target.toggleClass(toggleClasses);

  var addClass = $(this).data('add-classes');
  if (addClass)
    $target.addClass(addClass);

  var removeClass = $(this).data('remove-classes');
  if (removeClass)
    $target.removeClass(removeClass);

  return false;
});

<?php

function never_maybe_defined($foo) {
  if ($foo == 'something') {
    $maybeDefined = 42;
  }

  if (isset($maybeDefined)) {
    echo $maybeDefined;
  } else {
    echo "reachable";
    echo $maybeDefined/*!PHP3007!*/;    
  }

  echo $maybeDefined/*!PHP3007!*/;

  echo isset($maybeDefined) ? $maybeDefined : $maybeDefined/*!PHP3007!*/;

  if (isset($maybeDefined) || isset($neverDefined)) {
    echo $maybeDefined/*!PHP3007!*/;
  }

  if (isset($maybeDefined) && isset($neverDefined)) {
    echo $maybeDefined;
  }

  if (!isset($maybeDefined)) {
    echo $maybeDefined/*!PHP3007!*/;    
  }
}

function always_defined_integer() {
  $alwaysDefined = 42;

  if (isset(/*|integer|*/$alwaysDefined)) {
    echo /*|integer|*/$alwaysDefined;
  } else {
    echo "unreachable";/*!PHP3012!*/
    echo /*|null|*/$alwaysDefined;
  }

  echo $alwaysDefined;
}

function always_defined_int_null($x) {
  $alwaysDefined = $x ? 42 : null;

  if (isset(/*|integer|null*/$alwaysDefined)) {
    echo /*|integer|*/$alwaysDefined;
  } else {
    echo "reachable";
    echo /*|null|*/$alwaysDefined;
  }

  echo /*|integer|null*/$alwaysDefined;
}

function always_defined_mixed($alwaysDefined) {
  if (isset(/*|mixed|*/$alwaysDefined)) {

    if (isset(/*|mixed|*/$alwaysDefined)) {
      echo /*|mixed|*/$alwaysDefined;
    } else {
      echo "reachable";               // If the type is mixed, we were unable to propagate that it is never null
      echo /*|null|*/$alwaysDefined;
    }

    echo /*|mixed|*/$alwaysDefined;
  }
}

function always_defined_int_null_multiple($x) {
  /*|integer|null|*/$a = $x ? 42 : null;
  /*|integer|null|*/$b = $x ? 42 : null;

  if (isset($a, $b)) {
    echo /*|integer|*/$a;
    echo /*|integer|*/$b;
  } else {
    // We don't know exactly which check failed => can't constraint the type
    echo /*|integer|null|*/$a;
    echo /*|integer|null|*/$b;
  }
}